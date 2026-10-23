extern alias Analyzers;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Analyzers::CodeBoost.CodeAnalysis.Analyzers;
using CodeBoost.Performance;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace CodeBoost.Tests;

/// <summary>
/// Runs <see cref="MainAnalyzer"/> over source compiled in memory, as a consuming project's build runs it.
/// </summary>
public class CodeHealthReporterTests
{
    /// <summary>
    /// The id of the diagnostic for a pool-resettable member that nothing resets.
    /// </summary>
    private const string UnresetMemberDiagnosticId = "CODEHEALTHREPORTER01";

    /// <summary>
    /// A type that lists <see cref="IPoolResettable"/> itself has its unreset members reported.
    /// </summary>
    [Fact]
    public async Task ImplementingType_UnresetMember_IsReported()
    {
        const string Source = """
            using CodeBoost.CodeAnalysis;
            using CodeBoost.Performance;

            public class Thing : IPoolResettable
            {
                [PoolResettableMember]
                private int _count;

                public void Increment() => _count++;

                public void OnReturn() { }

                public void OnRent() { }
            }
            """;

        ImmutableArray<Diagnostic> diagnostics = await GetAnalyzerDiagnosticsAsync(Source);

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(UnresetMemberDiagnosticId, diagnostic.Id);
        Assert.Equal("_count", diagnostic.Location.SourceTree!.GetText().ToString(diagnostic.Location.SourceSpan));
    }

    /// <summary>
    /// A type that takes <see cref="IPoolResettable"/> from its base class has its own unreset members reported. The check read only the
    /// interfaces a type lists itself, so every type that inherits the interface went unchecked.
    /// </summary>
    [Fact]
    public async Task DerivedType_UnresetMember_IsReported()
    {
        const string Source = """
            using CodeBoost.CodeAnalysis;
            using CodeBoost.Performance;

            public class DerivedThing : BaseThing
            {
                [PoolResettableMember]
                private int _count;

                public void Increment() => _count++;
            }

            public class BaseThing : IPoolResettable
            {
                public virtual void OnReturn() { }

                public void OnRent() { }
            }
            """;

        ImmutableArray<Diagnostic> diagnostics = await GetAnalyzerDiagnosticsAsync(Source);

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(UnresetMemberDiagnosticId, diagnostic.Id);
        Assert.Equal("_count", diagnostic.Location.SourceTree!.GetText().ToString(diagnostic.Location.SourceSpan));
    }

    /// <summary>
    /// A derived type that resets its member in its override of <see cref="IPoolResettable.OnReturn"/> is not reported.
    /// </summary>
    [Fact]
    public async Task DerivedType_MemberResetInOverride_IsNotReported()
    {
        const string Source = """
            using CodeBoost.CodeAnalysis;
            using CodeBoost.Performance;

            public class DerivedThing : BaseThing
            {
                [PoolResettableMember]
                private int _count;

                public void Increment() => _count++;

                public override void OnReturn()
                {
                    base.OnReturn();

                    _count = 0;
                }
            }

            public class BaseThing : IPoolResettable
            {
                public virtual void OnReturn() { }

                public void OnRent() { }
            }
            """;

        ImmutableArray<Diagnostic> diagnostics = await GetAnalyzerDiagnosticsAsync(Source);

        Assert.Empty(diagnostics);
    }

    /// <summary>
    /// A generic base reached from a closed derived type is checked as its own declaration, whose members are the ones its method bodies
    /// reference. Checked as the closed type, its members are substituted copies that no reference matches, and every one was reported.
    /// </summary>
    [Fact]
    public async Task ClosedDerivedType_GenericBaseResettingItsMember_IsNotReported()
    {
        const string Source = """
            using CodeBoost.CodeAnalysis;
            using CodeBoost.Performance;

            public class ClosedThing : GenericThing<int>
            {
            }

            public class GenericThing<T0> : BaseThing
            {
                [PoolResettableMember]
                private T0 _value;

                public void Set(T0 value) => _value = value;

                public override void OnReturn()
                {
                    base.OnReturn();

                    _value = default;
                }
            }

            public class BaseThing : IPoolResettable
            {
                public virtual void OnReturn() { }

                public void OnRent() { }
            }
            """;

        ImmutableArray<Diagnostic> diagnostics = await GetAnalyzerDiagnosticsAsync(Source);

        Assert.Empty(diagnostics);
    }

    /// <summary>
    /// Compiles <paramref name="source"/> against the framework and CodeBoost, and returns what <see cref="MainAnalyzer"/> reports for it.
    /// </summary>
    /// <param name="source">The source to compile, which must compile without errors.</param>
    /// <returns>The analyzer's diagnostics.</returns>
    /// <remarks>
    /// Analysis runs without concurrency, so types are visited in source order and a test decides which type the analyzer reaches first.
    /// </remarks>
    private static async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(string source)
    {
        // The framework is every trusted assembly in the runtime's own folder, which leaves out the test's dependencies, the analyzers among them.
        string runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        string trustedPlatformAssemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        List<MetadataReference> metadataReferences = [MetadataReference.CreateFromFile(typeof(IPoolResettable).Assembly.Location)];

        foreach (string assemblyLocation in trustedPlatformAssemblies.Split(Path.PathSeparator))
        {
            if (string.Equals(Path.GetDirectoryName(assemblyLocation), runtimeDirectory, StringComparison.OrdinalIgnoreCase))
                metadataReferences.Add(MetadataReference.CreateFromFile(assemblyLocation));
        }

        CSharpCompilation compilation = CSharpCompilation.Create("AnalyzerSource", [CSharpSyntaxTree.ParseText(source)], metadataReferences, new(OutputKind.DynamicallyLinkedLibrary));

        foreach (Diagnostic compilerDiagnostic in compilation.GetDiagnostics())
            Assert.True(compilerDiagnostic.Severity is not DiagnosticSeverity.Error, compilerDiagnostic.ToString());

        CompilationWithAnalyzersOptions compilationWithAnalyzersOptions = new(new([]), onAnalyzerException: null, concurrentAnalysis: false, logAnalyzerExecutionTime: false);
        CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers([new MainAnalyzer()], compilationWithAnalyzersOptions);

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    }
}
