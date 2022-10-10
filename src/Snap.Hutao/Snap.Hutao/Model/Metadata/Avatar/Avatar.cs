// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Binding.Gacha.Abstraction;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Converter;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 角色
/// </summary>
public class Avatar : IStatisticsItemSource, ISummaryItemSource, INameQuality
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 体型
    /// </summary>
    public string Body { get; set; } = default!;

    /// <summary>
    /// 正面图标
    /// </summary>
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 侧面图标
    /// </summary>
    public string SideIcon { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 角色加入游戏时间
    /// </summary>
    public DateTimeOffset BeginTime { get; set; }

    /// <summary>
    /// 星级
    /// </summary>
    public ItemQuality Quality { get; set; }

    /// <summary>
    /// 武器类型
    /// </summary>
    public WeaponType Weapon { get; set; }

    /// <summary>
    /// 属性
    /// </summary>
    public PropertyInfo Property { get; set; } = default!;

    /// <summary>
    /// 技能
    /// </summary>
    public SkillDepot SkillDepot { get; set; } = default!;

    /// <summary>
    /// 好感信息/基本信息
    /// </summary>
    public FetterInfo FetterInfo { get; set; } = default!;

    /// <summary>
    /// 皮肤
    /// </summary>
    public IEnumerable<Costume> Costumes { get; set; } = default!;

    /// <summary>
    /// 转换为基础物品
    /// </summary>
    /// <returns>基础物品</returns>
    public ItemBase ToItemBase()
    {
        return new()
        {
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToIconUri(FetterInfo.VisionBefore),
            Quality = Quality,
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
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToIconUri(FetterInfo.VisionBefore),
            Quality = Quality,
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
