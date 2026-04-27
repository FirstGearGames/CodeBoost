using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeBoost.CodeAnalysis.Analyzers.Receivers;

/// <summary>
/// Routes incoming syntax-node analysis events to the appropriate CodeBoost receiver.
/// </summary>
public class CombinedReceiver
{
    /// <summary>
    /// The syntax kinds that this receiver subscribes to.
    /// </summary>
    public static readonly SyntaxKind[] SyntaxKinds =
    [
        SyntaxKind.ClassDeclaration,
        SyntaxKind.StructDeclaration,
    ];

    private readonly CodeHealthReporter _codeHealthReporter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedReceiver"/> class.
    /// </summary>
    /// <param name="codeHealthReporter">Reporter that should receive routed events.</param>
    public CombinedReceiver(CodeHealthReporter codeHealthReporter)
    {
        _codeHealthReporter = codeHealthReporter;
    }

    /// <summary>
    /// Handles a syntax-node analysis callback by dispatching it to the appropriate handler.
    /// </summary>
    /// <param name="context">Analysis context for the visited node.</param>
    public void OnVisitSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        SyntaxNode syntaxNode = context.Node;

        if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            _codeHealthReporter.HandleClassDeclarationSyntax(context, classDeclaration);
        else if (syntaxNode is StructDeclarationSyntax structDeclaration)
            _codeHealthReporter.HandleStructDeclarationSyntax(context, structDeclaration);
    }
}