using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Snap.Hutao.SourceGeneration.Reliquary;

[Generator(LanguageNames.CSharp)]
internal sealed class ReliquaryWeightConfigurationGenerator : IIncrementalGenerator
{
    private const string FileName = "ReliquaryWeightConfiguration.json";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<AdditionalText>> provider = context.AdditionalTextsProvider.Where(MatchFileName).Collect();

        context.RegisterSourceOutput(provider, GenerateReliquaryWeightConfiguration);
    }

    private static bool MatchFileName(AdditionalText text)
    {
        return Path.GetFileName(text.Path) == FileName;
    }

    private static void GenerateReliquaryWeightConfiguration(SourceProductionContext context, ImmutableArray<AdditionalText> texts)
    {
        AdditionalText jsonFile = texts.Single();

        string configurationJson = jsonFile.GetText(context.CancellationToken)!.ToString();
        Dictionary<string, ReliquaryWeightConfigurationMetadata> metadataMap =
            configurationJson.FromJson<Dictionary<string, ReliquaryWeightConfigurationMetadata>>()!;

        StringBuilder sourceBuilder = new StringBuilder().Append($$"""
            // Copyright (c) DGP Studio. All rights reserved.
            // Licensed under the MIT license.

            namespace Snap.Hutao.Service.AvatarInfo.Factory;

            /// <summary>
            /// 圣遗物评分权重配置
            /// </summary>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(ReliquaryWeightConfigurationGenerator)}}","1.0.0.0")]
            internal static class ReliquaryWeightConfiguration
            {
                /// <summary>
                /// 默认
                /// </summary>
                public static readonly AffixWeight Default = new(0, 100, 75, 0, 100, 100, 0, 55, 0);

                /// <summary>
                /// 词条权重
                /// </summary>
                public static readonly List<AffixWeight> AffixWeights = new()
                {

            """);

        foreach (KeyValuePair<string, ReliquaryWeightConfigurationMetadata> kvp in metadataMap.OrderBy(kvp => kvp.Key))
        {
            AppendAffixWeight(sourceBuilder, kvp.Key, kvp.Value);
        }

        sourceBuilder.Append($$"""
                };
            }
            """);

        context.AddSource("ReliquaryWeightConfiguration.g.cs", sourceBuilder.ToString());
    }

    private static void AppendAffixWeight(StringBuilder builder, string id, ReliquaryWeightConfigurationMetadata metadata)
    {
        StringBuilder lineBuilder = new StringBuilder()
            .Append("        new AffixWeight(").Append(id).Append(',')
            .Append(' ').Append(metadata.Hp).Append(',')
            .Append(' ').Append(metadata.Attack).Append(',')
            .Append(' ').Append(metadata.Defense).Append(',')
            .Append(' ').Append(metadata.CritRate).Append(',')
            .Append(' ').Append(metadata.CritHurt).Append(',')
            .Append(' ').Append(metadata.Mastery).Append(',')
            .Append(' ').Append(metadata.Recharge).Append(',')
            .Append(' ').Append(metadata.Healing).Append(')')
            .Append('.').Append(metadata.ElementType).Append("(").Append(metadata.ElementHurt).Append(')');

        if (metadata.PhysicialHurt != 0)
        {
            lineBuilder.Append(".Physical(").Append(metadata.PhysicialHurt).Append(')');
        }

        lineBuilder.Append(',');

        builder.AppendLine(lineBuilder.ToString());
    }

    private sealed class ReliquaryWeightConfigurationMetadata
    {
        [DataMember(Name = "hp")]
        public int Hp { get; set; }

        [DataMember(Name = "atk")]
        public int Attack { get; set; }

        [DataMember(Name = "def")]
        public int Defense { get; set; }

        [DataMember(Name = "cpct")]
        public int CritRate { get; set; }

        [DataMember(Name = "cdmg")]
        public int CritHurt { get; set; }

        [DataMember(Name = "mastery")]
        public int Mastery { get; set; }

        [DataMember(Name = "recharge")]
        public int Recharge { get; set; }

        [DataMember(Name = "heal")]
        public int Healing { get; set; }

        [DataMember(Name = "element")]
        public string ElementType { get; set; } = default!;

        [DataMember(Name = "dmg")]
        public int ElementHurt { get; set; }

        [DataMember(Name = "phy")]
        public int PhysicialHurt { get; set; }
    }
}