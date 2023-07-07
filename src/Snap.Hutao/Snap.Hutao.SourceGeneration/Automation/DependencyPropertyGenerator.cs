// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Snap.Hutao.SourceGeneration.Primitive;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Snap.Hutao.SourceGeneration.Automation;

[Generator(LanguageNames.CSharp)]
internal sealed class DependencyPropertyGenerator : IIncrementalGenerator
{
    private const string AttributeName = "Snap.Hutao.Core.Annotation.DependencyPropertyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<GeneratorSyntaxContext2>> commands =
            context.SyntaxProvider.CreateSyntaxProvider(FilterAttributedClasses, CommandMethod)
            .Where(GeneratorSyntaxContext2.NotNull)
            .Collect();

        context.RegisterImplementationSourceOutput(commands, GenerateDependencyPropertyImplementations);
    }

    private static bool FilterAttributedClasses(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.Modifiers.Count > 1
            && classDeclarationSyntax.HasAttributeLists();
    }

    private static GeneratorSyntaxContext2 CommandMethod(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.TryGetDeclaredSymbol(token, out INamedTypeSymbol? methodSymbol))
        {
            ImmutableArray<AttributeData> attributes = methodSymbol.GetAttributes();
            if (attributes.Any(data => data.AttributeClass!.ToDisplayString() == AttributeName))
            {
                return new(context, methodSymbol, attributes);
            }
        }

        return default;
    }

    private static void GenerateDependencyPropertyImplementations(SourceProductionContext production, ImmutableArray<GeneratorSyntaxContext2> context2s)
    {
        foreach (GeneratorSyntaxContext2 context2 in context2s.DistinctBy(c => c.Symbol.ToDisplayString()))
        {
            GenerateDependencyPropertyImplementation(production, context2);
        }
    }

    private static void GenerateDependencyPropertyImplementation(SourceProductionContext production, GeneratorSyntaxContext2 context2)
    {
        foreach (AttributeData propertyInfo in context2.Attributes.Where(attr => attr.AttributeClass!.ToDisplayString() == AttributeName))
        {
            ImmutableArray<TypedConstant> arguments = propertyInfo.ConstructorArguments;

            string propertyName = (string)arguments[0].Value!;
            string propertyType = arguments[0].Type!.ToDisplayString();
            string type = arguments[1].Value!.ToString();
            string defaultValue = arguments.Length > 2
                ? GetLiteralString(arguments[2])
                : "default";
            string className = context2.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            string code = $$"""
                using Microsoft.UI.Xaml;

                namespace {{context2.Symbol.ContainingNamespace}};

                partial class {{className}}
                {
                    private DependencyProperty {{propertyName}}Property =
                        DependencyProperty.Register(nameof({{propertyName}}), typeof({{type}}), typeof({{className}}), new PropertyMetadata(({{type}}){{defaultValue}}));

                    public {{type}} {{propertyName}}
                    {
                        get => ({{type}})GetValue({{propertyName}}Property);
                        set => SetValue({{propertyName}}Property, value);
                    }
                }
                """;

            string normalizedClassName = context2.Symbol.ToDisplayString().Replace('<', '{').Replace('>', '}');
            production.AddSource($"{normalizedClassName}.{propertyName}.g.cs", code);
        }
    }

    private static string GetLiteralString(TypedConstant typedConstant)
    {
        if (typedConstant.Value is bool boolValue)
        {
            return boolValue ? "true" : "false";
        }

        return typedConstant.Value!.ToString();
    }
}