// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Model.Metadata.Weapon;

/// <summary>
/// 武器的接口实现
/// </summary>
internal sealed partial class Weapon : IStatisticsItemSource, ISummaryItemSource, IItemSource, INameQuality, ICalculableSource<ICalculableWeapon>
{
    /// <summary>
    /// [非元数据] 搭配数据
    /// TODO:Add View suffix.
    /// </summary>
    [JsonIgnore]
    public WeaponCollocationView? Collocation { get; set; }

    /// <summary>
    /// [非元数据] 养成物品视图
    /// </summary>
    [JsonIgnore]
    public List<Material>? CultivationItemsView { get; set; }

    /// <inheritdoc cref="INameQuality.Quality" />
    [JsonIgnore]
    public QualityType Quality
    {
        get => RankLevel;
    }

    /// <summary>
    /// 最大等级
    /// </summary>
    internal uint MaxLevel { get => GetMaxLevelByQuality(Quality); }

    public static uint GetMaxLevelByQuality(QualityType quality)
    {
        return quality >= QualityType.QUALITY_BLUE ? 90U : 70U;
    }

    /// <inheritdoc/>
    public ICalculableWeapon ToCalculable()
    {
        return CalculableWeapon.From(this);
    }

    /// <summary>
    /// 转换为基础物品
    /// </summary>
    /// <returns>基础物品</returns>
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

    /// <summary>
    /// 转换到统计物品
    /// </summary>
    /// <param name="count">个数</param>
    /// <returns>统计物品</returns>
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

    /// <summary>
    /// 转换到简述统计物品
    /// </summary>
    /// <param name="lastPull">距上个五星</param>
    /// <param name="time">时间</param>
    /// <param name="isUp">是否为Up物品</param>
    /// <returns>简述统计物品</returns>
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
}