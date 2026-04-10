using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeBoost.CodeAnalysis.Analyzers.Receivers;

public class CombinedReceiver
{
    public static readonly SyntaxKind[] SyntaxKinds =
    [
        SyntaxKind.ClassDeclaration,
        SyntaxKind.StructDeclaration,
    ];

    private readonly CodeHealthReporter _codeHealthReporter;

    public CombinedReceiver(CodeHealthReporter codeHealthReporter)
    {
        _codeHealthReporter = codeHealthReporter;
    }

    public void OnVisitSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        SyntaxNode syntaxNode = context.Node;

        if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            _codeHealthReporter.HandleClassDeclarationSyntax(context, classDeclaration);
        else if (syntaxNode is StructDeclarationSyntax structDeclaration)
            _codeHealthReporter.HandleStructDeclarationSyntax(context, structDeclaration);
    }
}