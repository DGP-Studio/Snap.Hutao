// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 角色信息
/// </summary>
internal sealed class AvatarView : INameIconSide, ICalculableSource<ICalculableAvatar>
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
    public string Level { get => $"Lv.{LevelNumber}"; }

    /// <summary>
    /// 武器
    /// </summary>
    public WeaponView? Weapon { get; set; } = default!;

    /// <summary>
    /// 圣遗物列表
    /// </summary>
    public List<ReliquaryView> Reliquaries { get; set; } = default!;

    /// <summary>
    /// 命之座列表
    /// </summary>
    public List<ConstellationView> Constellations { get; set; } = default!;

    /// <summary>
    /// 激活的命之座个数
    /// </summary>
    public int ActivatedConstellationCount { get => Constellations.Where(c => c.IsActivated).Count(); }

    /// <summary>
    /// 技能列表
    /// </summary>
    public List<SkillView> Skills { get; set; } = default!;

    /// <summary>
    /// 属性
    /// </summary>
    public List<AvatarProperty> Properties { get; set; } = default!;

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