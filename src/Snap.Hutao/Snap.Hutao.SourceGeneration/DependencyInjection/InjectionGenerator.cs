// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Snap.Hutao.SourceGeneration.Primitive;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Snap.Hutao.SourceGeneration.DependencyInjection;

[Generator(LanguageNames.CSharp)]
internal sealed class InjectionGenerator : IIncrementalGenerator
{
    private const string AttributeName = "Snap.Hutao.Core.DependencyInjection.Annotation.InjectionAttribute";
    private const string InjectAsSingletonName = "Snap.Hutao.Core.DependencyInjection.Annotation.InjectAs.Singleton";
    private const string InjectAsTransientName = "Snap.Hutao.Core.DependencyInjection.Annotation.InjectAs.Transient";
    private const string InjectAsScopedName = "Snap.Hutao.Core.DependencyInjection.Annotation.InjectAs.Scoped";
    private const string CRLF = "\r\n";

    private static readonly DiagnosticDescriptor invalidInjectionDescriptor = new("SH101", "无效的 InjectAs 枚举值", "尚未支持生成 {0} 配置", "Quality", DiagnosticSeverity.Error, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<GeneratorSyntaxContext2>> injectionClasses = context.SyntaxProvider
            .CreateSyntaxProvider(FilterAttributedClasses, HttpClientClass)
            .Where(GeneratorSyntaxContext2.NotNull)
            .Collect();

        context.RegisterImplementationSourceOutput(injectionClasses, GenerateAddInjectionsImplementation);
    }

    private static bool FilterAttributedClasses(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0;
    }

    private static GeneratorSyntaxContext2 HttpClientClass(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, token) is INamedTypeSymbol classSymbol)
        {
            ImmutableArray<AttributeData> attributes = classSymbol.GetAttributes();
            if (attributes.Any(data => data.AttributeClass!.ToDisplayString() == AttributeName))
            {
                return new(context, classSymbol, attributes);
            }
        }

        return default;
    }

    private static void GenerateAddInjectionsImplementation(SourceProductionContext context, ImmutableArray<GeneratorSyntaxContext2> context2s)
    {
        StringBuilder sourceBuilder = new StringBuilder().Append($$"""
            // Copyright (c) DGP Studio. All rights reserved.
            // Licensed under the MIT license.

            namespace Snap.Hutao.Core.DependencyInjection;
            
            internal static partial class ServiceCollectionExtension
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InjectionGenerator)}}","1.0.0.0")]
                [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                public static partial IServiceCollection AddInjections(this IServiceCollection services)
                {
            """);

        FillUpWithAddServices(sourceBuilder, context, context2s);
        sourceBuilder.Append("""

                    return services;
                }
            }
            """);

        context.AddSource("ServiceCollectionExtension.g.cs", sourceBuilder.ToString());
    }

    private static void FillUpWithAddServices(StringBuilder sourceBuilder, SourceProductionContext production, ImmutableArray<GeneratorSyntaxContext2> contexts)
    {
        List<string> lines = new();
        StringBuilder lineBuilder = new();

        foreach (GeneratorSyntaxContext2 context in contexts)
        {
            lineBuilder.Clear().Append(CRLF);

            AttributeData injectionInfo = context.SingleAttributeWithName(AttributeName);
            ImmutableArray<TypedConstant> arguments = injectionInfo.ConstructorArguments;

            string injectAsName = arguments[0].ToCSharpString();
            switch (injectAsName)
            {
                case InjectAsSingletonName:
                    lineBuilder.Append("        services.AddSingleton<");
                    break;
                case InjectAsTransientName:
                    lineBuilder.Append("        services.AddTransient<");
                    break;
                case InjectAsScopedName:
                    lineBuilder.Append("        services.AddScoped<");
                    break;
                default:
                    production.ReportDiagnostic(Diagnostic.Create(invalidInjectionDescriptor, null, injectAsName));
                    break;
            }

            if (arguments.Length == 2)
            {
                lineBuilder.Append($"{arguments[1].Value}, ");
            }

            lineBuilder.Append($"{context.Symbol.ToDisplayString()}>();");

            lines.Add(lineBuilder.ToString());
        }

        foreach (string line in lines.OrderBy(x => x))
        {
            sourceBuilder.Append(line);
        }
    }
}