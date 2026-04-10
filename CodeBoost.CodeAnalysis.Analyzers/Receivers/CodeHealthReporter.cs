using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalysis.Extensions;
using CodeAnalysis.Finding;
using CodeAnalysis.Logging;
using CodeBoost.CodeAnalysis.Analyzers.Receivers.Diagnostics;
using CodeBoost.Performance;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeBoost.CodeAnalysis.Analyzers.Receivers;

public class CodeHealthReporter
{
    /// <summary>
    /// Full names of NamedTypes checked to be networked.
    /// </summary>
    private readonly HashSet<string> _typesCheckedFullName = new();
    /// <summary>
    /// Current CachedDiagnostics.
    /// </summary>
    private readonly List<Diagnostic> _cachedDiagnostics = [];
    private readonly string _poolResettableFullName;
    
    private readonly string _poolResettableMemberAttributeFullName;
    private readonly string _poolResettableMemberAttributeName;
    
    private readonly string _poolDisposableMemberAttributeFullName;
    private readonly string _poolDisposableMemberAttributeName;

    private readonly string _poolResettableMethodAttributeFullName;
    private readonly string _poolResettableMethodAttributeName;

    
    public CodeHealthReporter()
    {
        string? name;

        //IPoolResettable.
        name = typeof(IPoolResettable).FullName;
        if (name is null)
            OnFullNameNull(typeof(IPoolResettable));
        else
            _poolResettableFullName = name;

        //PoolResettableMemberAttribute.
        name = typeof(PoolResettableMemberAttribute).FullName;
        if (name is null)
            OnFullNameNull(typeof(PoolResettableMemberAttribute));
        else
            _poolResettableMemberAttributeFullName = name;

        _poolResettableMemberAttributeName = nameof(PoolResettableMemberAttribute);
        
        //PoolDisposableMemberAttribute.
        name = typeof(PoolDisposableMemberAttribute).FullName;
        if (name is null)
            OnFullNameNull(typeof(PoolDisposableMemberAttribute));
        else
            _poolDisposableMemberAttributeFullName = name;

        _poolDisposableMemberAttributeName = nameof(PoolDisposableMemberAttribute);
        
        //PoolDisposableMemberAttribute.
        name = typeof(PoolDisposableMemberAttribute).FullName;
        if (name is null)
            OnFullNameNull(typeof(PoolDisposableMemberAttribute));
        else
            _poolDisposableMemberAttributeFullName = name;

        _poolDisposableMemberAttributeName = nameof(PoolDisposableMemberAttribute);

        //PoolResettableMethodAttribute.
        name = typeof(PoolResettableMethodAttribute).FullName;
        if (name is null)
            OnFullNameNull(typeof(PoolResettableMethodAttribute));
        else
            _poolResettableMethodAttributeFullName = name;

        _poolResettableMethodAttributeName = nameof(PoolResettableMethodAttribute);
        
        void OnFullNameNull(Type lType)
        {
            string message = $"The FullName could not be found for Type [{lType.Name}]. [{nameof(CodeHealthReporter)}] will not execute properly.";
            _cachedDiagnostics.Add(Diagnostic.Create(DiagnosticRules.CodeHealthReporterError, Location.None, message));
        }
    }

    /// <summary>
    /// Returns if there are any CachedDiagnostics followed by purging existing entries.
    /// </summary>
    public bool TryPurgeCachedDiagnostics(out List<Diagnostic>? cachedDiagnostics)
    {
        if (_cachedDiagnostics.Count == 0)
        {
            cachedDiagnostics = null;
            return false;
        }

        cachedDiagnostics = _cachedDiagnostics.ToList();
        _cachedDiagnostics.Clear();

        return true;
    }

    public void HandleClassDeclarationSyntax(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclarationSyntax)
    {
        SemanticModel semanticModel = context.SemanticModel;
        if (semanticModel is null)
            return;

        HandleClassDeclarationSyntax(semanticModel, classDeclarationSyntax);
    }

    public void HandleClassDeclarationSyntax(SemanticModel semanticModel, ClassDeclarationSyntax classDeclarationSyntax)
    {
        if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
            return;

        AddNamedType(semanticModel, namedTypeSymbol, FindingFlags.Recursive);
    }

    public void HandleStructDeclarationSyntax(SyntaxNodeAnalysisContext context, StructDeclarationSyntax structDeclarationSyntax)
    {
        SemanticModel semanticModel = context.SemanticModel;
        if (semanticModel is null)
            return;

        HandleStructDeclarationSyntax(semanticModel, structDeclarationSyntax);
    }

    public void HandleStructDeclarationSyntax(SemanticModel semanticModel, StructDeclarationSyntax structDeclarationSyntax)
    {
        if (semanticModel.GetDeclaredSymbol(structDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
            return;

        AddNamedType(semanticModel, namedTypeSymbol, FindingFlags.Recursive);
    }

    /// <summary>
    /// Adds a named type to networked types if it meets findingFlags conditions.
    /// </summary>
    private void AddNamedType(SemanticModel semanticModel, INamedTypeSymbol namedTypeSymbol, FindingFlags findingFlags)
    {
        bool isRecursive = findingFlags.HasFlag(FindingFlags.Recursive);

        INamedTypeSymbol? currentNamedTypeSymbol = namedTypeSymbol;

        // Continue while namedType has value or until break.
        while (currentNamedTypeSymbol is not null)
        {
            // Already marked as networkType.
            if (!_typesCheckedFullName.Add(currentNamedTypeSymbol.GetTypeSymbolFullName()))
                break;

            if (!InspectForPoolResettable(semanticModel, namedTypeSymbol))
                break;

            currentNamedTypeSymbol = isRecursive ? currentNamedTypeSymbol.BaseType : null;
        }
    }

    /// <summary>
    /// Inspects members for IPoolResettable attributes.
    /// </summary>
    /// <returns>True if completed without errors.</returns>
    private bool InspectForPoolResettable(SemanticModel semanticModel, INamedTypeSymbol namedTypeSymbol)
    {
        // The Type does not utilize IPoolResettable; no further checks are needed.
        if (!namedTypeSymbol.TypeSymbolImplementsInterface(_poolResettableFullName))
            return true;

        IMethodSymbol? onReturnMethodSymbol = namedTypeSymbol.GetMethod(nameof(IPoolResettable.OnReturn));
        /* The OnReturn is not implemented. The user would already be
         * seeing an error from their IDE; we do not need to proceed until
         * after the user implements this method. */
        if (onReturnMethodSymbol is null)
            return true;

        List<ISymbol> poolResettableMemberSymbols = [];
        List<ISymbol> poolDisposableMemberSymbols = [];

        foreach (ISymbol symbol in namedTypeSymbol.GetMembers())
        {
            if (symbol.HasAttribute(SearchScope.Exact, _poolResettableMemberAttributeFullName, out _))
                poolResettableMemberSymbols.Add(symbol);

            if (symbol.HasAttribute(SearchScope.Exact, _poolDisposableMemberAttributeFullName, out _))
                poolDisposableMemberSymbols.Add(symbol);
        }

        /* There are not any members marked for resettable
         * nor disposable. */
        if (poolResettableMemberSymbols.Count == 0 && poolDisposableMemberSymbols.Count == 0)
            return true;

        const string DisposeName = nameof(IDisposable.Dispose);

        if (IterateMethodSymbol(onReturnMethodSymbol))
            return true;

        /* If here then poolResettableMembers still remain.
         *
         * Find all IMethodSymbols in the NamedTypeSymbol and
         * if the method has the PoolResettableMethodAttribute,
         * iterate for PoolResettableMember references. */
        List<IMethodSymbol> methodSymbols = namedTypeSymbol.GetMethodSymbols(requiredAccessibility: null);

        /* Iterate methods which have the PoolResettableMethod
         * attribute. */
        foreach (IMethodSymbol methodSymbol in methodSymbols)
        {
            if (!methodSymbol.HasAttribute(SearchScope.Exact, _poolResettableMethodAttributeFullName, out _))
                continue;

            if (IterateMethodSymbol(methodSymbol))
                return true;
        }

        /* Any symbols which remain in poolResettableMembers were
         * not found being referenced. */

        foreach (ISymbol poolResettableMemberSymbol in poolResettableMemberSymbols)
        {
            string message = $"Member declares [{_poolResettableMemberAttributeName}] but no references were found in the [{nameof(IPoolResettable.OnReturn)}] method nor within [{_poolResettableMethodAttributeName}] attributed methods.";
            _cachedDiagnostics.Add(Diagnostic.Create(DiagnosticRules.CodeHealthReporterError, poolResettableMemberSymbol.GetIdentifierLocation(), message));
        }

        foreach (ISymbol poolDisposableMemberSymbol in poolDisposableMemberSymbols)
        {
            string message = $"Member declares [{_poolDisposableMemberAttributeName}] but no references were found calling Dispose in the [{nameof(IPoolResettable.OnReturn)}] method nor within [{_poolResettableMethodAttributeName}] attributed methods.";
            _cachedDiagnostics.Add(Diagnostic.Create(DiagnosticRules.CodeHealthReporterError, poolDisposableMemberSymbol.GetIdentifierLocation(), message));
        }

        return false;

        /* Iterates a MethodSymbol and returns true if both
         * the poolResettableMemberSymbols and poolDisposableMemberSymbols
         * collections are empty. */
        bool IterateMethodSymbol(IMethodSymbol lMethodSymbol)
        {
            if (!lMethodSymbol.TryGetReferencedSymbols(semanticModel, out HashSet<ISymbol>? lReferencedSymbols))
            {
                string message = $"Referenced symbols were not found when executing [{nameof(InspectForPoolResettable)}].";
                _cachedDiagnostics.Add(Diagnostic.Create(DiagnosticRules.CodeHealthReporterError, namedTypeSymbol.GetIdentifierLocation(), message));

                return false;
            }

            /* Remove Symbols which are referenced from
             * poolResettableMemberSymbols. */
            if (poolResettableMemberSymbols.Count > 0)
            {
                foreach (ISymbol lReferencedSymbol in lReferencedSymbols!)
                    poolResettableMemberSymbols.Remove(lReferencedSymbol);
            }

            /* Remove Symbols which are referenced prior to
             * a Dispose call from poolDisposableMemberSymbols. */
            if (poolDisposableMemberSymbols.Count > 0)
            {
                // The last iterated ISymbol.
                ISymbol? previousReferencedSymbol = null;

                foreach (ISymbol lReferencedSymbol in lReferencedSymbols!)
                {
                    if (lReferencedSymbol is IMethodSymbol referencedMethodSymbol)
                    {
                        /* The Roslyn version which this uses will always present
                         * a variable calling a method immediately before the called
                         * method symbol.
                         *
                         * This behaviour is abused to store the calling variable
                         * before checking if the called method is Dispose.
                         *
                         * If at some point this no longer works the IDE will output
                         * an error to the user indicating Dispose is not called. Even
                         * if this were to become a problem the user-resolution is
                         * to remove the PoolDisposableMember attribute, and report the
                         * issue so it may be handled. */
                        if (previousReferencedSymbol is not null)
                        {
                            if (referencedMethodSymbol.Name.Equals(DisposeName))
                                poolDisposableMemberSymbols.Remove(previousReferencedSymbol);
                        }
                    }

                    previousReferencedSymbol = lReferencedSymbol;
                }
            }

            return poolResettableMemberSymbols.Count == 0 && poolDisposableMemberSymbols.Count == 0;
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void LogInformation(string message) => CodeAnalysisLogger.LogInformation($"[{nameof(CodeHealthReporter)}] {message}");
}