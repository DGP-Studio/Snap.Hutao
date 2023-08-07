// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Snap.Hutao.SourceGeneration.Primitive;
using System.Collections.Generic;
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
            string owner = context2.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            Dictionary<string, TypedConstant> namedArguments = propertyInfo.NamedArguments.ToDictionary();
            bool isAttached = namedArguments.TryGetValue("IsAttached", out TypedConstant constant) && (bool)constant.Value!;
            string register = isAttached ? "RegisterAttached" : "Register";

            ImmutableArray<TypedConstant> arguments = propertyInfo.ConstructorArguments;

            string propertyName = (string)arguments[0].Value!;
            string propertyType = arguments[1].Value!.ToString();
            string defaultValue = GetLiteralString(arguments.ElementAtOrDefault(2)) ?? "default";
            string propertyChangedCallback = arguments.ElementAtOrDefault(3) is { IsNull: false } arg3 ? $", {arg3.Value}" : string.Empty;

            string code;
            if (isAttached)
            {
                string objType = namedArguments.TryGetValue("AttachedType", out TypedConstant attachedType)
                    ? attachedType.Value!.ToString()
                    : "object";

                code = $$"""
                    using Microsoft.UI.Xaml;

                    namespace {{context2.Symbol.ContainingNamespace}};

                    partial class {{owner}}
                    {
                        private static readonly DependencyProperty {{propertyName}}Property =
                            DependencyProperty.RegisterAttached("{{propertyName}}", typeof({{propertyType}}), typeof({{owner}}), new PropertyMetadata(({{propertyType}}){{defaultValue}}{{propertyChangedCallback}}));

                        public static {{propertyType}} Get{{propertyName}}({{objType}} obj)
                        {
                            return obj?.GetValue({{propertyName}}Property) as Type;
                        }

                        public static void Set{{propertyName}}({{objType}} obj, {{propertyType}} value)
                        {
                            obj.SetValue({{propertyName}}Property, value);
                        }
                    }
                    """;
            }
            else
            {
                code = $$"""
                    using Microsoft.UI.Xaml;

                    namespace {{context2.Symbol.ContainingNamespace}};

                    partial class {{owner}}
                    {
                        private static readonly DependencyProperty {{propertyName}}Property =
                            DependencyProperty.Register(nameof({{propertyName}}), typeof({{propertyType}}), typeof({{owner}}), new PropertyMetadata(({{propertyType}}){{defaultValue}}{{propertyChangedCallback}}));

                        public {{propertyType}} {{propertyName}}
                        {
                            get => ({{propertyType}})GetValue({{propertyName}}Property);
                            set => SetValue({{propertyName}}Property, value);
                        }
                    }
                    """;
            }

            string normalizedClassName = context2.Symbol.ToDisplayString().Replace('<', '{').Replace('>', '}');
            production.AddSource($"{normalizedClassName}.{propertyName}.g.cs", code);
        }
    }

    private static string? GetLiteralString(TypedConstant typedConstant)
    {
        if (typedConstant.IsNull)
        {
            return default;
        }

        if (typedConstant.Value is bool boolValue)
        {
            return boolValue ? "true" : "false";
        }

        string result = typedConstant.Value!.ToString();
        if (string.IsNullOrEmpty(result))
        {
            return default;
        }

        return result;
    }
}