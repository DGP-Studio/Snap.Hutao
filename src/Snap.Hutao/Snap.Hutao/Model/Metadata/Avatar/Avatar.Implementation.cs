// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 角色的接口实现部分
/// </summary>
internal partial class Avatar : IStatisticsItemSource, ISummaryItemSource, INameQuality, ICalculableSource<ICalculableAvatar>
{
    /// <summary>
    /// [非元数据] 搭配数据
    /// </summary>
    [JsonIgnore]
    public ComplexAvatarCollocation? Collocation { get; set; }

    /// <summary>
    /// [非元数据] 烹饪奖励
    /// </summary>
    [JsonIgnore]
    public CookBonusView? CookBonusView { get; set; }

    /// <summary>
    /// 养成物品视图
    /// </summary>
    public List<Material>? CultivationItemsView { get; set; }

    /// <summary>
    /// 最大等级
    /// </summary>
    public int MaxLevel { get => 90; }

    /// <inheritdoc/>
    public ICalculableAvatar ToCalculable()
    {
        return new CalculableAvatar(this);
    }

    /// <summary>
    /// 转换为基础物品
    /// </summary>
    /// <returns>基础物品</returns>
    public Binding.Item ToItemBase()
    {
        return new()
        {
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToIconUri(FetterInfo.VisionBefore),
            Quality = Quality,
        };
    }

    /// <inheritdoc/>
    public StatisticsItem ToStatisticsItem(int count)
    {
        return new()
        {
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToIconUri(FetterInfo.VisionBefore),
            Quality = Quality,

            Count = count,
        };
    }

    /// <inheritdoc/>
    public SummaryItem ToSummaryItem(int lastPull, DateTimeOffset time, bool isUp)
    {
        return new()
        {
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToIconUri(FetterInfo.VisionBefore),
            Quality = Quality,

            Time = time,
            LastPull = lastPull,
            IsUp = isUp,
        };
    }
}