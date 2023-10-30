using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Snap.Hutao.SourceGeneration.Primitive;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Snap.Hutao.SourceGeneration.Enum;

[Generator(LanguageNames.CSharp)]
internal class LocalizedEnumGenerator : IIncrementalGenerator
{
    private const string AttributeName = "Snap.Hutao.Resource.Localization.LocalizationAttribute";
    private const string LocalizationKeyName = "Snap.Hutao.Resource.Localization.LocalizationKeyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<GeneratorSyntaxContext2> localizationEnums = context.SyntaxProvider
            .CreateSyntaxProvider(FilterAttributedEnums, LocalizationEnum)
            .Where(GeneratorSyntaxContext2.NotNull);

        context.RegisterSourceOutput(localizationEnums, GenerateGetLocalizedDescriptionImplementation);
    }

    private static bool FilterAttributedEnums(SyntaxNode node, CancellationToken token)
    {
        return node is EnumDeclarationSyntax enumDeclarationSyntax
            && enumDeclarationSyntax.HasAttributeLists();
    }

    private static GeneratorSyntaxContext2 LocalizationEnum(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, token) is INamedTypeSymbol enumSymbol)
        {
            ImmutableArray<AttributeData> attributes = enumSymbol.GetAttributes();
            if (attributes.Any(data => data.AttributeClass!.ToDisplayString() == AttributeName))
            {
                return new(context, enumSymbol, attributes);
            }
        }

        return default;
    }

    private static void GenerateGetLocalizedDescriptionImplementation(SourceProductionContext context, GeneratorSyntaxContext2 context2)
    {
        StringBuilder sourceBuilder = new StringBuilder().Append($$"""
            // Copyright (c) DGP Studio. All rights reserved.
            // Licensed under the MIT license.
            
            using System.Globalization;

            namespace Snap.Hutao.Resource.Localization;

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(LocalizedEnumGenerator)}}", "1.0.0.0")]
            internal static class {{context2.Symbol.Name}}Extension
            {
                /// <summary>
                /// 获取本地化的描述
                /// </summary>
                /// <param name="value">枚举值</param>
                /// <returns>本地化的描述</returns>
                public static string GetLocalizedDescription(this {{context2.Symbol}} value)
                {
                    string key = value switch
                    {

            """);

        FillUpWithSwitchBranches(sourceBuilder, context2);

        sourceBuilder.Append($$"""
                        _ => string.Empty,
                    };

                    if (string.IsNullOrEmpty(key))
                    {
                        return Enum.GetName(value);
                    }
                    else
                    {
                        return SH.ResourceManager.GetString(key, CultureInfo.CurrentCulture);
                    }
                }

                /// <summary>
                /// 获取本地化的描述
                /// </summary>
                /// <param name="value">枚举值</param>
                /// <returns>本地化的描述</returns>
                [return:MaybeNull]
                public static string GetLocalizedDescriptionOrDefault(this {{context2.Symbol}} value)
                {
                    string key = value switch
                    {

            """);

        FillUpWithSwitchBranches(sourceBuilder, context2);

        sourceBuilder.Append($$"""
                        _ => string.Empty,
                    };

                    return SH.ResourceManager.GetString(key, CultureInfo.CurrentCulture);
                }
            }
            """);

        context.AddSource($"{context2.Symbol.Name}Extension.g.cs", sourceBuilder.ToString());
    }

    private static void FillUpWithSwitchBranches(StringBuilder sourceBuilder, GeneratorSyntaxContext2 context)
    {
        IEnumerable<IFieldSymbol> fields = context.Symbol.GetMembers()
            .Where(m => m.Kind == SymbolKind.Field)
            .Cast<IFieldSymbol>();

        foreach (IFieldSymbol fieldSymbol in fields)
        {
            AttributeData? localizationKeyInfo = fieldSymbol.GetAttributes()
                .SingleOrDefault(data => data.AttributeClass!.ToDisplayString() == LocalizationKeyName);
            if (localizationKeyInfo != null)
            {
                sourceBuilder
                    .Append("            ")
                    .Append(fieldSymbol)
                    .Append(" => \"")
                    .Append(localizationKeyInfo.ConstructorArguments[0].Value)
                    .AppendLine("\",");
            }
        }
    }
}
