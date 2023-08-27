// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Snap.Hutao.SourceGeneration.Primitive;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Snap.Hutao.SourceGeneration.Automation;

[Generator(LanguageNames.CSharp)]
internal sealed class ConstructorGenerator : IIncrementalGenerator
{
    private const string AttributeName = "Snap.Hutao.Core.Annotation.ConstructorGeneratedAttribute";

    //private static readonly DiagnosticDescriptor genericTypeNotSupportedDescriptor = new("SH102", "Generic type is not supported to generate .ctor", "Type [{0}] is not supported", "Quality", DiagnosticSeverity.Error, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<GeneratorSyntaxContext2>> injectionClasses =
            context.SyntaxProvider.CreateSyntaxProvider(FilterAttributedClasses, ConstructorGeneratedClass)
            .Where(GeneratorSyntaxContext2.NotNull)
            .Collect();

        context.RegisterSourceOutput(injectionClasses, GenerateConstructorImplementations);
    }

    private static bool FilterAttributedClasses(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.Modifiers.Count > 1
            && classDeclarationSyntax.HasAttributeLists();
    }

    private static GeneratorSyntaxContext2 ConstructorGeneratedClass(GeneratorSyntaxContext context, CancellationToken token)
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

    private static void GenerateConstructorImplementations(SourceProductionContext production, ImmutableArray<GeneratorSyntaxContext2> context2s)
    {
        foreach (GeneratorSyntaxContext2 context2 in context2s.DistinctBy(c => c.Symbol.ToDisplayString()))
        {
            GenerateConstructorImplementation(production, context2);
        }
    }

    private static void GenerateConstructorImplementation(SourceProductionContext production, GeneratorSyntaxContext2 context2)
    {
        AttributeData constructorInfo = context2.SingleAttribute(AttributeName);

        bool resolveHttpClient = constructorInfo.HasNamedArgumentWith<bool>("ResolveHttpClient", value => value);
        bool callBaseConstructor = constructorInfo.HasNamedArgumentWith<bool>("CallBaseConstructor", value => value);
        string httpclient = resolveHttpClient ? ", System.Net.Http.HttpClient httpClient" : string.Empty;

        FieldValueAssignmentOptions options = new(resolveHttpClient, callBaseConstructor);

        StringBuilder sourceBuilder = new StringBuilder().Append($$"""
            namespace {{context2.Symbol.ContainingNamespace}};

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(ConstructorGenerator)}}", "1.0.0.0")]
            partial class {{context2.Symbol.ToDisplayString(SymbolDisplayFormats.QualifiedNonNullableFormat)}}
            {
                public {{context2.Symbol.Name}}(System.IServiceProvider serviceProvider{{httpclient}}){{(options.CallBaseConstructor ? " : base(serviceProvider)" : string.Empty)}}
                {

            """);

        FillUpWithFieldValueAssignment(sourceBuilder, context2, options);

        sourceBuilder.Append("""
                }
            }
            """);

        string normalizedClassName = context2.Symbol.ToDisplayString().Replace('<', '{').Replace('>', '}');
        production.AddSource($"{normalizedClassName}.ctor.g.cs", sourceBuilder.ToString());
    }

    private static void FillUpWithFieldValueAssignment(StringBuilder builder, GeneratorSyntaxContext2 context2, FieldValueAssignmentOptions options)
    {
        IEnumerable<IFieldSymbol> fields = context2.Symbol.GetMembers()
            .Where(m => m.Kind == SymbolKind.Field)
            .OfType<IFieldSymbol>();

        foreach (IFieldSymbol fieldSymbol in fields)
        {
            bool shoudSkip = false;
            foreach (SyntaxReference syntaxReference in fieldSymbol.DeclaringSyntaxReferences)
            {
                if (syntaxReference.GetSyntax() is VariableDeclaratorSyntax declarator)
                {
                    if (declarator.Initializer is not null)
                    {
                        // Skip field with initializer
                        builder.Append("        // Skip field with initializer: ").AppendLine(fieldSymbol.Name);
                        shoudSkip = true;
                        break;
                    }
                }
            }

            if (shoudSkip)
            {
                continue;
            }

            if (fieldSymbol.IsReadOnly && !fieldSymbol.IsStatic)
            {
                switch (fieldSymbol.Type.ToDisplayString())
                {
                    case "System.IServiceProvider":
                        builder
                            .Append("        this.")
                            .Append(fieldSymbol.Name)
                            .AppendLine(" = serviceProvider;");
                        break;

                    case "System.Net.Http.HttpClient":
                        if (options.ResolveHttpClient)
                        {
                            builder
                                .Append("        this.")
                                .Append(fieldSymbol.Name)
                                .AppendLine(" = httpClient;");
                        }
                        else
                        {
                            builder
                                .Append("        this.")
                                .Append(fieldSymbol.Name)
                                .Append(" = serviceProvider.GetRequiredService<System.Net.Http.IHttpClientFactory>().CreateClient(nameof(")
                                .Append(context2.Symbol.Name)
                                .AppendLine("));");
                        }
                        break;

                    default:
                        builder
                            .Append("        this.")
                            .Append(fieldSymbol.Name)
                            .Append(" = serviceProvider.GetRequiredService<")
                            .Append(fieldSymbol.Type)
                            .AppendLine(">();");
                        break;
                }
            }
        }

        foreach (INamedTypeSymbol interfaceSymbol in context2.Symbol.Interfaces)
        {
            if (interfaceSymbol.Name == "IRecipient")
            {
                builder
                    .Append("        CommunityToolkit.Mvvm.Messaging.IMessengerExtensions.Register<")
                    .Append(interfaceSymbol.TypeArguments[0])
                    .AppendLine(">(serviceProvider.GetRequiredService<CommunityToolkit.Mvvm.Messaging.IMessenger>(), this);");
            }
        }
    }

    private readonly struct FieldValueAssignmentOptions
    {
        public readonly bool ResolveHttpClient;
        public readonly bool CallBaseConstructor;

        public FieldValueAssignmentOptions(bool resolveHttpClient, bool callBaseConstructor)
        {
            ResolveHttpClient = resolveHttpClient;
            CallBaseConstructor = callBaseConstructor;
        }
    }
}