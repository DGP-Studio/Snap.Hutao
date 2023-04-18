using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Snap.Hutao.SourceGeneration;

/// <summary>
/// 通用分析器
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class UniversalAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor typeInternalDescriptor = new("SH001", "Type should be internal", "Type [{0}] should be internal", "Quality", DiagnosticSeverity.Info, true);
    private static readonly DiagnosticDescriptor readOnlyStructRefDescriptor = new("SH002", "ReadOnly struct should be passed with ref-like key word", "ReadOnly Struct [{0}] should be passed with ref-like key word", "Quality", DiagnosticSeverity.Info, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            return new DiagnosticDescriptor[]
    {
        typeInternalDescriptor,
        readOnlyStructRefDescriptor,
    }.ToImmutableArray();
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(CompilationStart);
    }

    private void CompilationStart(CompilationStartAnalysisContext context)
    {
        SyntaxKind[] types = new SyntaxKind[]
        {
            SyntaxKind.ClassDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.EnumDeclaration
        };

        context.RegisterSyntaxNodeAction(HandleTypeDeclaration, types);

        context.RegisterSyntaxNodeAction(CollectReadOnlyStruct, SyntaxKind.StructDeclaration);
        context.RegisterSyntaxNodeAction(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        BaseTypeDeclarationSyntax syntax = (BaseTypeDeclarationSyntax)context.Node;

        bool privateExists = false;
        bool internalExists = false;
        bool fileExists = false;

        foreach (SyntaxToken token in syntax.Modifiers)
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
            Diagnostic diagnostic = Diagnostic.Create(typeInternalDescriptor, location, syntax.Identifier);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private readonly HashSet<string> readOnlyStructs = new();

    private void CollectReadOnlyStruct(SyntaxNodeAnalysisContext context)
    {
        StructDeclarationSyntax structSyntax = (StructDeclarationSyntax)context.Node;

        if (structSyntax.Modifiers.Any(token => token.IsKind(SyntaxKind.ReadOnlyKeyword)))
        {
            if (context.SemanticModel.GetDeclaredSymbol(structSyntax) is INamedTypeSymbol symbol)
            {
                readOnlyStructs.Add(symbol.ToDisplayString());
            }
        }
    }

    private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        MethodDeclarationSyntax methodSyntax = (MethodDeclarationSyntax)context.Node;

        // 跳过异步方法，因为异步方法无法使用 ref in out
        if (methodSyntax.Modifiers.Any(token => token.IsKind(SyntaxKind.AsyncKeyword)))
        {
            return;
        }

        // 跳过方法定义 如 接口
        if (methodSyntax.Body == null)
        {
            return;
        }

        foreach (ParameterSyntax parameter in methodSyntax.ParameterList.Parameters)
        {
            if (context.SemanticModel.GetDeclaredSymbol(parameter) is IParameterSymbol symbol)
            {
                if (readOnlyStructs.Contains(symbol.Type.ToDisplayString()) && symbol.RefKind == RefKind.None)
                {
                    Location location = parameter.GetLocation();
                    Diagnostic diagnostic = Diagnostic.Create(readOnlyStructRefDescriptor, location, symbol.Type);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}