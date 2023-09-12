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
internal sealed class CommandGenerator : IIncrementalGenerator
{
    public const string AttributeName = "Snap.Hutao.Core.Annotation.CommandAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<GeneratorSyntaxContext2<IMethodSymbol>>> commands =
            context.SyntaxProvider.CreateSyntaxProvider(FilterAttributedMethods, CommandMethod)
            .Where(GeneratorSyntaxContext2<IMethodSymbol>.NotNull)
            .Collect();

        context.RegisterImplementationSourceOutput(commands, GenerateCommandImplementations);
    }

    private static bool FilterAttributedMethods(SyntaxNode node, CancellationToken token)
    {
        return node is MethodDeclarationSyntax methodDeclarationSyntax
            && methodDeclarationSyntax.Parent is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.Modifiers.Count > 1
            && methodDeclarationSyntax.HasAttributeLists();
    }

    private static GeneratorSyntaxContext2<IMethodSymbol> CommandMethod(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.TryGetDeclaredSymbol(token, out IMethodSymbol? methodSymbol))
        {
            ImmutableArray<AttributeData> attributes = methodSymbol.GetAttributes();
            if (attributes.Any(data => data.AttributeClass!.ToDisplayString() == AttributeName))
            {
                return new(context, methodSymbol, attributes);
            }
        }

        return default;
    }

    private static void GenerateCommandImplementations(SourceProductionContext production, ImmutableArray<GeneratorSyntaxContext2<IMethodSymbol>> context2s)
    {
        foreach (GeneratorSyntaxContext2<IMethodSymbol> context2 in context2s.DistinctBy(c => c.Symbol.ToDisplayString()))
        {
            GenerateCommandImplementation(production, context2);
        }
    }

    private static void GenerateCommandImplementation(SourceProductionContext production, GeneratorSyntaxContext2<IMethodSymbol> context2)
    {
        INamedTypeSymbol classSymbol = context2.Symbol.ContainingType;

        AttributeData commandInfo = context2.SingleAttribute(AttributeName);
        string commandName = (string)commandInfo.ConstructorArguments[0].Value!;

        string commandType = context2.Symbol.ReturnType.IsOrInheritsFrom("System.Threading.Tasks.Task")
            ? "AsyncRelayCommand"
            : "RelayCommand";

        string genericParameter = context2.Symbol.Parameters.ElementAtOrDefault(0) is IParameterSymbol parameter
            ? $"<{parameter.Type.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNonNullableFormat)}>"
            : string.Empty;

        string concurrentExecution = commandInfo.HasNamedArgumentWith<bool>("AllowConcurrentExecutions", value => value)
            ? ", AsyncRelayCommandOptions.AllowConcurrentExecutions"
            : string.Empty;

        string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        string code = $$"""
            using CommunityToolkit.Mvvm.Input;

            namespace {{classSymbol.ContainingNamespace}};

            partial class {{className}}
            {
                private ICommand _{{commandName}};

                public ICommand {{commandName}}
                {
                    get => _{{commandName}} ??= new {{commandType}}{{genericParameter}}({{context2.Symbol.Name}}{{concurrentExecution}});
                }
            }
            """;

        string normalizedClassName = classSymbol.ToDisplayString().Replace('<', '{').Replace('>', '}');
        production.AddSource($"{normalizedClassName}.{commandName}.g.cs", code);
    }
}