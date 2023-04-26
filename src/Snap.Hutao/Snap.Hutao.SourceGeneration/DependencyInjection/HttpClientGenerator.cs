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

    private const string DefaultName = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.HttpClientConfiguration.Default";
    private const string XRpcName = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.HttpClientConfiguration.XRpc";
    private const string XRpc2Name = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.HttpClientConfiguration.XRpc2";
    private const string XRpc3Name = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.HttpClientConfiguration.XRpc3";

    private const string PrimaryHttpMessageHandlerAttributeName = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.PrimaryHttpMessageHandlerAttribute";
    private const string UseDynamicSecretAttributeName = "Snap.Hutao.Web.Hoyolab.DynamicSecret.UseDynamicSecretAttribute";
    private const string CRLF = "\r\n";

    private static readonly DiagnosticDescriptor invalidConfigurationDescriptor = new("SH100", "无效的 HttpClientConfiguration", "尚未支持生成 {0} 配置", "Quality", DiagnosticSeverity.Error, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<GeneratorSyntaxContext2>> injectionClasses =
            context.SyntaxProvider.CreateSyntaxProvider(FilterAttributedClasses, HttpClientClass)
            .Where(GeneratorSyntaxContext2.NotNull)
            .Collect();

        context.RegisterImplementationSourceOutput(injectionClasses, GenerateAddHttpClientsImplementation);
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
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(HttpClientGenerator)}}","1.0.0.0")]
                [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
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

        foreach (GeneratorSyntaxContext2 context in contexts)
        {
            lineBuilder.Clear().Append(CRLF);
            lineBuilder.Append(@"        services.AddHttpClient<");

            AttributeData httpClientData = context.SingleAttributeWithName(AttributeName);
            ImmutableArray<TypedConstant> arguments = httpClientData.ConstructorArguments;

            if (arguments.Length == 2)
            {
                lineBuilder.Append($"{arguments[1].Value}, ");
            }

            lineBuilder.Append($"{context.Symbol.ToDisplayString()}>(");

            string configurationName = arguments[0].ToCSharpString();
            switch (configurationName)
            {
                case DefaultName:
                    lineBuilder.Append("DefaultConfiguration)");
                    break;
                case XRpcName:
                    lineBuilder.Append("XRpcConfiguration)");
                    break;
                case XRpc2Name:
                    lineBuilder.Append("XRpc2Configuration)");
                    break;
                case XRpc3Name:
                    lineBuilder.Append("XRpc3Configuration)");
                    break;
                default:
                    production.ReportDiagnostic(Diagnostic.Create(invalidConfigurationDescriptor, null, configurationName));
                    break;
            }

            if (context.SingleOrDefaultAttributeWithName(PrimaryHttpMessageHandlerAttributeName) is AttributeData handlerData)
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

            if (context.HasAttributeWithName(UseDynamicSecretAttributeName))
            {
                lineBuilder.Append(".AddHttpMessageHandler<DynamicSecretHandler>()");
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