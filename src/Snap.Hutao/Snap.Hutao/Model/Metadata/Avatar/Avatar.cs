// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Generic;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 角色
/// </summary>
public class Avatar
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
    /// 好感信息
    /// </summary>
    public FetterInfo FetterInfo { get; set; } = default!;

    /// <summary>
    /// 皮肤
    /// </summary>
    public IEnumerable<Costume> Costumes { get; set; } = default!;
}