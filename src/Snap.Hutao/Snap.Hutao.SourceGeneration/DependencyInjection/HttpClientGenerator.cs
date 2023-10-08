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
internal sealed class HttpClientGenerator : IIncrementalGenerator
{
    private const string AttributeName = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.HttpClientAttribute";

    private const string HttpClientConfiguration = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.HttpClientConfiguration.";
    private const string PrimaryHttpMessageHandlerAttributeName = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.PrimaryHttpMessageHandlerAttribute";
    private const string CRLF = "\r\n";

    private static readonly DiagnosticDescriptor injectionShouldOmitDescriptor = new("SH201", "Injection 特性可以省略", "HttpClient 特性已将 {0} 注册为 Transient 服务", "Quality", DiagnosticSeverity.Warning, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<GeneratorSyntaxContext2>> injectionClasses = context.SyntaxProvider
            .CreateSyntaxProvider(FilterAttributedClasses, HttpClientClass)
            .Where(GeneratorSyntaxContext2.NotNull)
            .Collect();

        context.RegisterImplementationSourceOutput(injectionClasses, GenerateAddHttpClientsImplementation);
    }

    private static bool FilterAttributedClasses(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.HasAttributeLists();
    }

    private static GeneratorSyntaxContext2 HttpClientClass(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.TryGetDeclaredSymbol(token, out INamedTypeSymbol? classSymbol))
        {
            ImmutableArray<AttributeData> attributes = classSymbol.GetAttributes();
            if (attributes.Any(data => data.AttributeClass!.ToDisplayString() == AttributeName))
            {
                return new(context, classSymbol, attributes);
            }
        }

        return default;
    }

    private static void GenerateAddHttpClientsImplementation(SourceProductionContext context, ImmutableArray<GeneratorSyntaxContext2> context2s)
    {
        StringBuilder sourceBuilder = new StringBuilder().Append($$"""
            // Copyright (c) DGP Studio. All rights reserved.
            // Licensed under the MIT license.

            using Snap.Hutao.Web.Hoyolab.DynamicSecret;
            using System.Net.Http;
            
            namespace Snap.Hutao.Core.DependencyInjection;
            
            internal static partial class IocHttpClientConfiguration
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(HttpClientGenerator)}}", "1.0.0.0")]
                public static partial IServiceCollection AddHttpClients(this IServiceCollection services)
                {
            """);

        FillUpWithAddHttpClient(sourceBuilder, context, context2s);

        sourceBuilder.Append("""

                    return services;
                }
            }
            """);

        context.AddSource("IocHttpClientConfiguration.g.cs", sourceBuilder.ToString());
    }

    private static void FillUpWithAddHttpClient(StringBuilder sourceBuilder, SourceProductionContext production, ImmutableArray<GeneratorSyntaxContext2> contexts)
    {
        List<string> lines = new();
        StringBuilder lineBuilder = new();

        foreach (GeneratorSyntaxContext2 context in contexts.DistinctBy(c => c.Symbol.ToDisplayString()))
        {
            if (context.SingleOrDefaultAttribute(InjectionGenerator.AttributeName) is AttributeData injectionData)
            {
                if (injectionData.ConstructorArguments[0].ToCSharpString() == InjectionGenerator.InjectAsTransientName)
                {
                    if (injectionData.ConstructorArguments.Length < 2)
                    {
                        production.ReportDiagnostic(Diagnostic.Create(injectionShouldOmitDescriptor, context.Context.Node.GetLocation(), context.Context.Node));
                    }
                }
            }

            lineBuilder.Clear().Append(CRLF);
            lineBuilder.Append(@"        services.AddHttpClient<");

            AttributeData httpClientData = context.SingleAttribute(AttributeName);
            ImmutableArray<TypedConstant> arguments = httpClientData.ConstructorArguments;

            if (arguments.Length == 2)
            {
                lineBuilder.Append($"{arguments[1].Value}, ");
            }

            lineBuilder.Append($"{context.Symbol.ToDisplayString()}>(");
            lineBuilder.Append(arguments[0].ToCSharpString().Substring(HttpClientConfiguration.Length)).Append("Configuration)");

            if (context.SingleOrDefaultAttribute(PrimaryHttpMessageHandlerAttributeName) is AttributeData handlerData)
            {
                ImmutableArray<KeyValuePair<string, TypedConstant>> properties = handlerData.NamedArguments;
                lineBuilder.Append(@".ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() {");

                foreach (KeyValuePair<string, TypedConstant> property in properties)
                {
                    lineBuilder.Append(' ');
                    lineBuilder.Append(property.Key);
                    lineBuilder.Append(" = ");
                    lineBuilder.Append(property.Value.ToCSharpString());
                    lineBuilder.Append(',');
                }

                lineBuilder.Append(" })");
            }

            lineBuilder.Append(';');

            lines.Add(lineBuilder.ToString());
        }

        foreach (string line in lines.OrderBy(x => x))
        {
            sourceBuilder.Append(line);
        }
    }
}