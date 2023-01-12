// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 角色信息
/// </summary>
public class Avatar : ICalculableSource<ICalculableAvatar>
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public Uri Icon { get; set; } = default!;

    /// <summary>
    /// 侧面图标
    /// </summary>
    public Uri SideIcon { get; set; } = default!;

    /// <summary>
    /// 名片
    /// </summary>
    public Uri NameCard { get; set; } = default!;

    /// <summary>
    /// 星级
    /// </summary>
    public ItemQuality Quality { get; set; }

    /// <summary>
    /// 元素类型
    /// </summary>
    public ElementType Element { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public string Level { get; set; } = default!;

    /// <summary>
    /// 武器
    /// </summary>
    public Weapon? Weapon { get; set; } = default!;

    /// <summary>
    /// 圣遗物列表
    /// </summary>
    public List<Reliquary> Reliquaries { get; set; } = default!;

    /// <summary>
    /// 命之座列表
    /// </summary>
    public List<Constellation> Constellations { get; set; } = default!;

    /// <summary>
    /// 技能列表
    /// </summary>
    public List<Skill> Skills { get; set; } = default!;

    /// <summary>
    /// 属性
    /// </summary>
    public List<Pair2<string, string, string?>> Properties { get; set; } = default!;

    /// <summary>
    /// 评分
    /// </summary>
    public string Score { get; set; } = default!;

    /// <summary>
    /// 双爆评分
    /// </summary>
    public string CritScore { get; set; } = default!;

    /// <summary>
    /// 好感度等级
    /// </summary>
    public int FetterLevel { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    internal AvatarId Id { get; set; }

    /// <summary>
    /// 等级数字
    /// </summary>
    internal int LevelNumber { get; set; }

    /// <inheritdoc/>
    public ICalculableAvatar ToCalculable()
    {
        return new CalculableAvatar(this);
    }
}