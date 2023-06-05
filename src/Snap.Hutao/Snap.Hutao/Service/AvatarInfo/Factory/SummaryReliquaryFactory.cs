// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Format;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.Web.Enka.Model;
using System.Runtime.InteropServices;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataReliquaryAffix = Snap.Hutao.Model.Metadata.Reliquary.ReliquarySubAffix;
using ModelAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using PropertyReliquary = Snap.Hutao.ViewModel.AvatarProperty.ReliquaryView;

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
        List<ReliquarySubProperty> subProperty = equip.Reliquary!.AppendPropIdList.EmptyIfNull().SelectList(CreateSubProperty);

        int affixCount = GetSecondaryAffixCount(reliquary);

        PropertyReliquary result = new()
        {
            // NameIconDescription
            Name = reliquary.Name,
            Icon = RelicIconConverter.IconNameToUri(reliquary.Icon),
            Description = reliquary.Description,

            // EquipBase
            Level = $"+{equip.Reliquary.Level - 1}",
            Quality = reliquary.RankLevel,
        };

        if (subProperty.Count > 0)
        {
            Span<ReliquarySubProperty> span = CollectionsMarshal.AsSpan(subProperty);
            result.PrimarySubProperties = new(span[..^affixCount].ToArray());
            result.SecondarySubProperties = new(span[^affixCount..].ToArray());
            result.ComposedSubProperties = equip.Flat.ReliquarySubstats!.SelectList(CreateComposedSubProperty);
            ReliquaryMainAffixLevel relicLevel = metadataContext.ReliqueryLevels.Single(r => r.Level == equip.Reliquary!.Level && r.Rank == reliquary.RankLevel);
            FightProperty property = metadataContext.IdRelicMainPropMap[equip.Reliquary.MainPropId];

            result.MainProperty = FightPropertyFormat.ToNameValue(property, relicLevel.PropertyMap[property]);
            result.Score = ScoreReliquary(property, reliquary, relicLevel, subProperty);
        }

        return result;
    }

    private int GetSecondaryAffixCount(MetadataReliquary reliquary)
    {
        // 强化词条个数
        return (reliquary.RankLevel, equip.Reliquary!.Level.Value) switch
        {
            (QualityType.QUALITY_ORANGE, > 20U) => 5,
            (QualityType.QUALITY_ORANGE, > 16U) => 4,
            (QualityType.QUALITY_ORANGE, > 12U) => 3,
            (QualityType.QUALITY_ORANGE, > 8U) => 2,
            (QualityType.QUALITY_ORANGE, > 4U) => 1,
            (QualityType.QUALITY_ORANGE, _) => 0,

            (QualityType.QUALITY_PURPLE, > 16U) => 4,
            (QualityType.QUALITY_PURPLE, > 12U) => 3,
            (QualityType.QUALITY_PURPLE, > 8U) => 2,
            (QualityType.QUALITY_PURPLE, > 4U) => 1,
            (QualityType.QUALITY_PURPLE, _) => 0,

            (QualityType.QUALITY_BLUE, > 12U) => 3,
            (QualityType.QUALITY_BLUE, > 8U) => 2,
            (QualityType.QUALITY_BLUE, > 4U) => 1,
            (QualityType.QUALITY_BLUE, _) => 0,

            (QualityType.QUALITY_GREEN, > 4U) => 1,
            (QualityType.QUALITY_GREEN, _) => 0,

            (QualityType.QUALITY_WHITE, > 4U) => 1,
            (QualityType.QUALITY_WHITE, _) => 0,

            _ => 0,
        };
    }

    private float ScoreReliquary(FightProperty property, MetadataReliquary reliquary, ReliquaryMainAffixLevel relicLevel, List<ReliquarySubProperty> subProperties)
    {
        // 沙 杯 头
        // equip.Flat.EquipType is EquipType.EQUIP_SHOES or EquipType.EQUIP_RING or EquipType.EQUIP_DRESS
        if ((int)equip.Flat.EquipType > 3)
        {
            AffixWeight weightConfig = GetAffixWeightForAvatarId();
            ReliquaryMainAffixLevel maxRelicLevel = metadataContext.ReliqueryLevels.Where(r => r.Rank == reliquary.RankLevel).MaxBy(r => r.Level)!;

            float percent = relicLevel.PropertyMap[property] / maxRelicLevel.PropertyMap[property];
            float baseScore = 8 * percent * weightConfig.GetValueOrDefault(property);

            float score = subProperties.Sum(p => p.Score);
            return ((score + baseScore) / 1700) * 66;
        }
        else
        {
            float score = subProperties.Sum(p => p.Score);
            return (score / 900) * 66;
        }
    }

    private AffixWeight GetAffixWeightForAvatarId()
    {
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

    [SuppressMessage("", "SH002")]
    private ReliquarySubProperty CreateSubProperty(ReliquarySubAffixId appendPropId)
    {
        MetadataReliquaryAffix affix = metadataContext.IdReliquarySubAffixMap[appendPropId];
        FightProperty property = affix.Type;

        return new(
            property.GetLocalizedDescription(),
            FightPropertyFormat.FormatValue(property, affix.Value),
            ScoreSubAffix(appendPropId));
    }

    private float ScoreSubAffix(in ReliquarySubAffixId appendId)
    {
        MetadataReliquaryAffix affix = metadataContext.IdReliquarySubAffixMap[appendId];

        AffixWeight weightConfig = GetAffixWeightForAvatarId();
        float weight = weightConfig.GetValueOrDefault(affix.Type) / 100F;

        // 小字词条，转换到等效百分比计算
        if (affix.Type is FightProperty.FIGHT_PROP_HP or FightProperty.FIGHT_PROP_ATTACK or FightProperty.FIGHT_PROP_DEFENSE)
        {
            // 等效百分比 [ 当前小字词条 / 角色基本属性 ]
            float equalPercent = affix.Value / avatarInfo.FightPropMap[affix.Type - 1];

            // 获取对应百分比词条权重
            weight = weightConfig.GetValueOrDefault(affix.Type + 1) / 100F;

            // 最大同属性百分比数值 最大同属性百分比Id 第四五位是战斗属性位
            MetadataReliquaryAffix maxPercentAffix = metadataContext.IdReliquarySubAffixMap[SummaryHelper.GetAffixMaxId(appendId + 10U)];
            float equalScore = equalPercent / maxPercentAffix.Value;

            return weight * equalScore * 100;
        }

        return weight * SummaryHelper.GetPercentSubAffixScore(appendId);
    }
}