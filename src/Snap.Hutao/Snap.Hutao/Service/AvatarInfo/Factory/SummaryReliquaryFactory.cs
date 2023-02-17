// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Annotation;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Web.Enka.Model;
using System.Runtime.InteropServices;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataReliquaryAffix = Snap.Hutao.Model.Metadata.Reliquary.ReliquaryAffix;
using ModelAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using PropertyReliquary = Snap.Hutao.Model.Binding.AvatarProperty.ReliquaryView;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 圣遗物工厂
/// </summary>
[HighQuality]
internal sealed class SummaryReliquaryFactory
{
    private readonly SummaryMetadataContext metadataContext;
    private readonly ModelAvatarInfo avatarInfo;
    private readonly Web.Enka.Model.Equip equip;

    /// <summary>
    /// 构造一个新的圣遗物工厂
    /// </summary>
    /// <param name="metadataContext">元数据上下文</param>
    /// <param name="avatarInfo">角色信息</param>
    /// <param name="equip">圣遗物</param>
    public SummaryReliquaryFactory(SummaryMetadataContext metadataContext, ModelAvatarInfo avatarInfo, Web.Enka.Model.Equip equip)
    {
        this.metadataContext = metadataContext;
        this.avatarInfo = avatarInfo;
        this.equip = equip;
    }

    /// <summary>
    /// 构造圣遗物
    /// </summary>
    /// <returns>圣遗物</returns>
    public PropertyReliquary CreateReliquary()
    {
        MetadataReliquary reliquary = metadataContext.Reliquaries.Single(r => r.Ids.Contains(equip.ItemId));
        List<ReliquarySubProperty> subProperty = equip.Reliquary!.AppendPropIdList.EmptyIfNull().Select(CreateSubProperty).ToList();

        int affixCount = GetSecondaryAffixCount(reliquary);
        if (subProperty.Count == 0)
        {
            return new()
            {
                // NameIconDescription
                Name = reliquary.Name,
                Icon = RelicIconConverter.IconNameToUri(reliquary.Icon),
                Description = reliquary.Description,

                // EquipBase
                Level = $"+{equip.Reliquary.Level - 1}",
                Quality = reliquary.RankLevel,
            };
        }

        Span<ReliquarySubProperty> span = CollectionsMarshal.AsSpan(subProperty);
        List<ReliquarySubProperty> primary = new(span[..^affixCount].ToArray());
        List<ReliquarySubProperty> secondary = new(span[^affixCount..].ToArray());

        List<ReliquarySubProperty> composed = equip.Flat.ReliquarySubstats!.Select(CreateComposedSubProperty).ToList();

        ReliquaryLevel relicLevel = metadataContext.ReliqueryLevels.Single(r => r.Level == equip.Reliquary!.Level && r.Quality == reliquary.RankLevel);
        FightProperty property = metadataContext.IdRelicMainPropMap[equip.Reliquary.MainPropId];

        return new()
        {
            // NameIconDescription
            Name = reliquary.Name,
            Icon = RelicIconConverter.IconNameToUri(reliquary.Icon),
            Description = reliquary.Description,

            // EquipBase
            Level = $"+{equip.Reliquary.Level - 1}",
            Quality = reliquary.RankLevel,
            MainProperty = new(property.GetLocalizedDescription(), Model.Metadata.Converter.PropertyDescriptor.FormatValue(property, relicLevel.Properties[property])),

            // Reliquary
            ComposedSubProperties = composed,
            PrimarySubProperties = primary,
            SecondarySubProperties = secondary,
            Score = ScoreReliquary(property, reliquary, relicLevel, subProperty),
        };
    }

    private int GetSecondaryAffixCount(MetadataReliquary reliquary)
    {
        // 强化词条个数
        return (reliquary.RankLevel, equip.Reliquary!.Level) switch
        {
            (ItemQuality.QUALITY_ORANGE, > 20) => 5,
            (ItemQuality.QUALITY_ORANGE, > 16) => 4,
            (ItemQuality.QUALITY_ORANGE, > 12) => 3,
            (ItemQuality.QUALITY_ORANGE, > 8) => 2,
            (ItemQuality.QUALITY_ORANGE, > 4) => 1,
            (ItemQuality.QUALITY_ORANGE, _) => 0,

            (ItemQuality.QUALITY_PURPLE, > 16) => 4,
            (ItemQuality.QUALITY_PURPLE, > 12) => 3,
            (ItemQuality.QUALITY_PURPLE, > 8) => 2,
            (ItemQuality.QUALITY_PURPLE, > 4) => 1,
            (ItemQuality.QUALITY_PURPLE, _) => 0,

            (ItemQuality.QUALITY_BLUE, > 12) => 3,
            (ItemQuality.QUALITY_BLUE, > 8) => 2,
            (ItemQuality.QUALITY_BLUE, > 4) => 1,
            (ItemQuality.QUALITY_BLUE, _) => 0,

            (ItemQuality.QUALITY_GREEN, > 4) => 1,
            (ItemQuality.QUALITY_GREEN, _) => 0,

            (ItemQuality.QUALITY_WHITE, > 4) => 1,
            (ItemQuality.QUALITY_WHITE, _) => 0,

            _ => 0,
        };
    }

    private double ScoreReliquary(FightProperty property, MetadataReliquary reliquary, ReliquaryLevel relicLevel, List<ReliquarySubProperty> subProperties)
    {
        // 沙 杯 头
        if (equip.Flat.EquipType is EquipType.EQUIP_SHOES or EquipType.EQUIP_RING or EquipType.EQUIP_DRESS)
        {
            AffixWeight weightConfig = GetAffixWeightForAvatarId();
            ReliquaryLevel maxRelicLevel = metadataContext.ReliqueryLevels.Where(r => r.Quality == reliquary.RankLevel).MaxBy(r => r.Level)!;

            double percent = relicLevel.Properties[property] / maxRelicLevel.Properties[property];
            double baseScore = 8 * percent * weightConfig.GetValueOrDefault(property, 0);

            double score = subProperties.Sum(p => p.Score);
            return ((score + baseScore) / 1700) * 66;
        }
        else
        {
            double score = subProperties.Sum(p => p.Score);
            return (score / 900) * 66;
        }
    }

    private AffixWeight GetAffixWeightForAvatarId()
    {
        // TODO: more score support
        return ReliquaryWeightConfiguration.AffixWeights.FirstOrDefault(w => w.AvatarId == avatarInfo.AvatarId, ReliquaryWeightConfiguration.Default);
    }

    private ReliquarySubProperty CreateComposedSubProperty(ReliquarySubstat substat)
    {
        FormatMethod method = substat.AppendPropId.GetFormatMethod();
        string valueFormatted = method switch
        {
            FormatMethod.Integer => Math.Round((double)substat.StatValue, MidpointRounding.AwayFromZero).ToString(),
            FormatMethod.Percent => $"{substat.StatValue}%",
            _ => substat.StatValue.ToString(),
        };

        return new(substat.AppendPropId.GetLocalizedDescription(), valueFormatted, 0);
    }

    private ReliquarySubProperty CreateSubProperty(int appendPropId)
    {
        MetadataReliquaryAffix affix = metadataContext.IdReliquaryAffixMap[appendPropId];
        FightProperty property = affix.Type;

        double score = ScoreSubAffix(appendPropId);
        return new(property.GetLocalizedDescription(), Model.Metadata.Converter.PropertyDescriptor.FormatValue(property, affix.Value), score);
    }

    private double ScoreSubAffix(int appendId)
    {
        MetadataReliquaryAffix affix = metadataContext.IdReliquaryAffixMap[appendId];

        AffixWeight weightConfig = GetAffixWeightForAvatarId();
        double weight = weightConfig.GetValueOrDefault(affix.Type) / 100D;

        // 小字词条，转换到等效百分比计算
        if (affix.Type is FightProperty.FIGHT_PROP_HP or FightProperty.FIGHT_PROP_ATTACK or FightProperty.FIGHT_PROP_DEFENSE)
        {
            // 等效百分比 [ 当前小字词条 / 角色基本属性 ]
            double equalPercent = affix.Value / avatarInfo.FightPropMap[affix.Type - 1];

            // 获取对应百分比词条权重
            weight = weightConfig.GetValueOrDefault(affix.Type + 1) / 100D;

            // 最大同属性百分比数值 最大同属性百分比Id 第四五位是战斗属性位
            MetadataReliquaryAffix maxPercentAffix = metadataContext.IdReliquaryAffixMap[SummaryHelper.GetAffixMaxId(appendId + 10)];
            double equalScore = equalPercent / maxPercentAffix.Value;

            return weight * equalScore * 100;
        }

        return weight * SummaryHelper.GetPercentSubAffixScore(appendId);
    }
}