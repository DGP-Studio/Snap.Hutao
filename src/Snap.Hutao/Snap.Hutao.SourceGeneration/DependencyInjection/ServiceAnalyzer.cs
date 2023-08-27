using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Snap.Hutao.SourceGeneration.Primitive;
using System.Collections.Immutable;
using System.Linq;

namespace Snap.Hutao.SourceGeneration.DependencyInjection;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class ServiceAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor NonSingletonUseServiceProviderDescriptor = new("SH301", "Non Singleton service should avoid direct use of IServiceProvider", "Non Singleton service should avoid direct use of IServiceProvider", "Quality", DiagnosticSeverity.Info, true);
    private static readonly DiagnosticDescriptor SingletonServiceCaptureNonSingletonServiceDescriptor = new("SH302", "Singleton service should avoid keep reference of non singleton service", "Singleton service should avoid keep reference of non singleton service", "Quality", DiagnosticSeverity.Info, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get => new DiagnosticDescriptor[]
    {
        NonSingletonUseServiceProviderDescriptor,
        SingletonServiceCaptureNonSingletonServiceDescriptor,
    }.ToImmutableArray();
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(CompilationStart);
    }

    private static void CompilationStart(CompilationStartAnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(HandleNonSingletonUseServiceProvider, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(HandleSingletonServiceCaptureNonSingletonService, SyntaxKind.ClassDeclaration);
    }

    private static void HandleNonSingletonUseServiceProvider(SyntaxNodeAnalysisContext context)
    {
        ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        if (classDeclarationSyntax.HasAttributeLists())
        {
            INamedTypeSymbol? classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
            if (classSymbol is not null)
            {
                foreach (AttributeData attributeData in classSymbol.GetAttributes())
                {
                    if (attributeData.AttributeClass!.ToDisplayString() is InjectionGenerator.AttributeName)
                    {
                        string serviceType = attributeData.ConstructorArguments[0].ToCSharpString();
                        if (serviceType is InjectionGenerator.InjectAsTransientName or InjectionGenerator.InjectAsScopedName)
                        {
                            HandleNonSingletonUseServiceProviderActual(context, classSymbol);
                        }
                    }
                }
            }
        }
    }

    private static void HandleNonSingletonUseServiceProviderActual(SyntaxNodeAnalysisContext context, INamedTypeSymbol classSymbol)
    {
        ISymbol? symbol = classSymbol.GetMembers().Where(m => m is IFieldSymbol f && f.Type.ToDisplayString() == "System.IServiceProvider").SingleOrDefault();

        if (symbol is not null)
        {
            Diagnostic diagnostic = Diagnostic.Create(NonSingletonUseServiceProviderDescriptor, symbol.Locations.FirstOrDefault());
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void HandleSingletonServiceCaptureNonSingletonService(SyntaxNodeAnalysisContext context)
    {
        //classSymbol.GetMembers().Where(m => m is IFieldSymbol { IsReadOnly: true, DeclaredAccessibility: Accessibility.Private } f);
    }
}
