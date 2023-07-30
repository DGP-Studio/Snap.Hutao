using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Snap.Hutao.SourceGeneration.Primitive;
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
    private static readonly DiagnosticDescriptor useValueTaskIfPossibleDescriptor = new("SH003", "Use ValueTask instead of Task whenever possible", "Use ValueTask instead of Task", "Quality", DiagnosticSeverity.Info, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            return new DiagnosticDescriptor[]
            {
                typeInternalDescriptor,
                readOnlyStructRefDescriptor,
                useValueTaskIfPossibleDescriptor,
            }.ToImmutableArray();
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(CompilationStart);
    }

    private static void CompilationStart(CompilationStartAnalysisContext context)
    {
        SyntaxKind[] types = new SyntaxKind[]
        {
            SyntaxKind.ClassDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.EnumDeclaration
        };

        context.RegisterSyntaxNodeAction(HandleTypeShouldBeInternal, types);

        context.RegisterSyntaxNodeAction(HandleMethodParameterShouldUseRefLikeKeyword, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(HandleMethodReturnTypeShouldUseValueTaskInsteadOfTask, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(HandleConstructorParameterShouldUseRefLikeKeyword, SyntaxKind.ConstructorDeclaration);
    }

    private static void HandleTypeShouldBeInternal(SyntaxNodeAnalysisContext context)
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

    private static void HandleMethodReturnTypeShouldUseValueTaskInsteadOfTask(SyntaxNodeAnalysisContext context)
    {
        MethodDeclarationSyntax methodSyntax = (MethodDeclarationSyntax)context.Node;
        IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodSyntax)!;

        // 跳过重载方法
        if (methodSyntax.Modifiers.Any(token => token.IsKind(SyntaxKind.OverrideKeyword)))
        {
            return;
        }

        if (methodSymbol.ReturnType.IsOrInheritsFrom("System.Threading.Tasks.Task"))
        {
            Location location = methodSyntax.ReturnType.GetLocation();
            Diagnostic diagnostic = Diagnostic.Create(useValueTaskIfPossibleDescriptor, location);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void HandleMethodParameterShouldUseRefLikeKeyword(SyntaxNodeAnalysisContext context)
    {
        MethodDeclarationSyntax methodSyntax = (MethodDeclarationSyntax)context.Node;
        IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodSyntax)!;
        if (methodSymbol.ReturnType.IsOrInheritsFrom("System.Threading.Tasks.Task"))
        {
            return;
        }

        if (methodSymbol.ReturnType.IsOrInheritsFrom("System.Threading.Tasks.ValueTask"))
        {
            return;
        }

        // 跳过异步方法，因为异步方法无法使用 ref in out
        if (methodSyntax.Modifiers.Any(token => token.IsKind(SyntaxKind.AsyncKeyword)))
        {
            return;
        }

        // 跳过重载方法
        if (methodSyntax.Modifiers.Any(token => token.IsKind(SyntaxKind.OverrideKeyword)))
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
                if (IsBuiltInType(symbol.Type))
                {
                    continue;
                }

                // 跳过 CancellationToken
                if (symbol.Type.ToDisplayString() == "System.Threading.CancellationToken")
                {
                    continue;
                }

                if (symbol.Type.IsReadOnly && symbol.RefKind == RefKind.None)
                {
                    Location location = parameter.GetLocation();
                    Diagnostic diagnostic = Diagnostic.Create(readOnlyStructRefDescriptor, location, symbol.Type);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    private static void HandleConstructorParameterShouldUseRefLikeKeyword(SyntaxNodeAnalysisContext context)
    {
        ConstructorDeclarationSyntax constructorSyntax = (ConstructorDeclarationSyntax)context.Node;

        foreach (ParameterSyntax parameter in constructorSyntax.ParameterList.Parameters)
        {
            if (context.SemanticModel.GetDeclaredSymbol(parameter) is IParameterSymbol symbol)
            {
                if (IsBuiltInType(symbol.Type))
                {
                    continue;
                }

                // 跳过 CancellationToken
                if (symbol.Type.ToDisplayString() == "System.Threading.CancellationToken")
                {
                    continue;
                }

                if (symbol.Type.IsReadOnly && symbol.RefKind == RefKind.None)
                {
                    Location location = parameter.GetLocation();
                    Diagnostic diagnostic = Diagnostic.Create(readOnlyStructRefDescriptor, location, symbol.Type);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    private static bool IsBuiltInType(ITypeSymbol symbol)
    {
        return symbol.SpecialType switch
        {
            SpecialType.System_Boolean => true,
            SpecialType.System_Char => true,
            SpecialType.System_SByte => true,
            SpecialType.System_Byte => true,
            SpecialType.System_Int16 => true,
            SpecialType.System_UInt16 => true,
            SpecialType.System_Int32 => true,
            SpecialType.System_UInt32 => true,
            SpecialType.System_Int64 => true,
            SpecialType.System_UInt64 => true,
            SpecialType.System_Decimal => true,
            SpecialType.System_Single => true,
            SpecialType.System_Double => true,
            SpecialType.System_IntPtr => true,
            SpecialType.System_UIntPtr => true,
            _ => false,
        };
    }
}