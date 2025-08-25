// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.ViewModel.GachaLog;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Weapon;

internal sealed partial class Weapon : IDefaultIdentity<WeaponId>,
    INameQualityAccess,
    IStatisticsItemConvertible,
    ISummaryItemConvertible,
    IItemConvertible,
    ICalculableSource<ICalculableWeapon>,
    ICultivationItemsAccess,
    IPropertyValuesProvider
{
    public required WeaponId Id { get; init; }

    public required PromoteId PromoteId { get; init; }

    public required uint Sort { get; init; }

    public required WeaponType WeaponType { get; init; }

    public required QualityType RankLevel { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string Icon { get; init; }

    public required string AwakenIcon { get; init; }

    public required WeaponTypeValueCollection GrowCurves { get; init; }

    public NameDescriptions? Affix { get; init; }

    public required ImmutableArray<MaterialId> CultivationItems { get; init; }

    [JsonIgnore]
    public WeaponCollocationView? CollocationView { get; set; }

    [JsonIgnore]
    public List<Material>? CultivationItemsView { get; set; }

    [JsonIgnore]
    public QualityType Quality
    {
        get => RankLevel;
    }

    internal uint MaxLevel { get => GetMaxLevelByQuality(Quality); }

    public static uint GetMaxLevelByQuality(QualityType quality)
    {
        return quality >= QualityType.QUALITY_BLUE ? 90U : 70U;
    }

    public ICalculableWeapon ToCalculable()
    {
        return CalculableWeapon.From(this);
    }

    public TItem ToItem<TItem>()
        where TItem : Model.Item, new()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = EquipIconConverter.IconNameToUri(Icon),
            Badge = WeaponTypeIconConverter.WeaponTypeToIconUri(WeaponType),
            Quality = RankLevel,
        };
    }

    public StatisticsItem ToStatisticsItem(int count)
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = EquipIconConverter.IconNameToUri(Icon),
            Badge = WeaponTypeIconConverter.WeaponTypeToIconUri(WeaponType),
            Quality = RankLevel,
            Count = count,
        };
    }

    public SummaryItem ToSummaryItem(int lastPull, in DateTimeOffset time, bool isUp)
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = EquipIconConverter.IconNameToUri(Icon),
            Badge = WeaponTypeIconConverter.WeaponTypeToIconUri(WeaponType),
            Time = time,
            Quality = RankLevel,
            LastPull = lastPull,
            IsUp = isUp,
        };
    }
}