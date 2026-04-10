using System.Collections.Generic;
using System.Collections.Immutable;
using CodeBoost.CodeAnalysis.Analyzers.Receivers;
using CodeBoost.CodeAnalysis.Analyzers.Receivers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeBoost.CodeAnalysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MainAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticRules.CodeHealthReporterWarning,
            DiagnosticRules.CodeHealthReporterError);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            CodeHealthReporter codeHealthReporter = new();
            CombinedReceiver combinedReceiver = new(codeHealthReporter);

            compilationContext.RegisterSyntaxNodeAction(syntaxContext => combinedReceiver.OnVisitSyntaxNode(syntaxContext), CombinedReceiver.SyntaxKinds);

            compilationContext.RegisterCompilationEndAction(compilationEndContext =>
            {
                if (codeHealthReporter.TryPurgeCachedDiagnostics(out List<Diagnostic>? codeHealthReportCachedDiagnostics))
                    IterateDiagnostics(codeHealthReportCachedDiagnostics!);

                /* Iterates a Diagnostic collection. This method is nested
                 * to support future receivers. */
                void IterateDiagnostics(List<Diagnostic> diagnostics)
                {
                    foreach (Diagnostic diagnostic in diagnostics)
                        compilationEndContext.ReportDiagnostic(diagnostic);
                }
            });
        });
    }
}
