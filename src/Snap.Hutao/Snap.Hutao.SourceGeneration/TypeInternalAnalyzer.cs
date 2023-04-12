using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Snap.Hutao.SourceGeneration;

/// <summary>
/// 类型应为内部分析器
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class TypeInternalAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor typeInternalDescriptor = new("SH001", "Type should be internal", "Type should be internal", "Quality", DiagnosticSeverity.Info, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(typeInternalDescriptor);
    

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(CompilationStart);
    }

    private void CompilationStart(CompilationStartAnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(HandleSyntax<ClassDeclarationSyntax>, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(HandleSyntax<InterfaceDeclarationSyntax>, SyntaxKind.InterfaceDeclaration);
        context.RegisterSyntaxNodeAction(HandleSyntax<StructDeclarationSyntax>, SyntaxKind.StructDeclaration);
        context.RegisterSyntaxNodeAction(HandleSyntax<EnumDeclarationSyntax>, SyntaxKind.EnumDeclaration);
    }

    private void HandleSyntax<TSyntax>(SyntaxNodeAnalysisContext classSyntax)
        where TSyntax : BaseTypeDeclarationSyntax
    {
        TSyntax syntax = (TSyntax)classSyntax.Node;

        bool privateExists = false;
        bool internalExists = false;
        bool fileExists = false;

        foreach(SyntaxToken token in syntax.Modifiers)
        {
            if (token.IsKind(SyntaxKind.PrivateKeyword))
            {
                privateExists = true;
            }

            if (token.IsKind(SyntaxKind.InternalKeyword))
            {
                internalExists = true;
            }

            if (token.IsKind(SyntaxKind.FileKeyword))
            {
                fileExists = true;
            }
        }

        if (!privateExists && !internalExists && !fileExists)
        {
            Location location = syntax.Identifier.GetLocation();
            Diagnostic diagnostic = Diagnostic.Create(typeInternalDescriptor, location);
            classSyntax.ReportDiagnostic(diagnostic);
        }
    }
}