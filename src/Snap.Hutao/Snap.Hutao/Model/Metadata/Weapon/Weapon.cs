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

namespace Snap.Hutao.Model.Metadata.Weapon;

[HighQuality]
internal sealed partial class Weapon : INameQualityAccess,
    IStatisticsItemConvertible,
    ISummaryItemConvertible,
    IItemConvertible,
    ICalculableSource<ICalculableWeapon>,
    ICultivationItemsAccess,
    IAdvancedCollectionViewItem
{
    public WeaponId Id { get; set; }

    public PromoteId PromoteId { get; set; }

    public uint Sort { get; set; }

    public WeaponType WeaponType { get; set; }

    public QualityType RankLevel { get; set; }

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string Icon { get; set; } = default!;

    public string AwakenIcon { get; set; } = default!;

    public List<WeaponTypeValue> GrowCurves { get; set; } = default!;

    public NameDescriptions? Affix { get; set; } = default!;

    public List<MaterialId> CultivationItems { get; set; } = default!;

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

    public Model.Item ToItem()
    {
        return new()
        {
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
            Name = Name,
            Icon = EquipIconConverter.IconNameToUri(Icon),
            Badge = WeaponTypeIconConverter.WeaponTypeToIconUri(WeaponType),
            Time = time,
            Quality = RankLevel,
            LastPull = lastPull,
            IsUp = isUp,
        };
    }

    public object? GetPropertyValue(string propertyName)
    {
        return propertyName switch
        {
            _ => default,
        };
    }
}