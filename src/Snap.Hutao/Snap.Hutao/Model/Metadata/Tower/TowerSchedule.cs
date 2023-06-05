// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Tower;

/// <summary>
/// 深渊 计划
/// </summary>
internal sealed class TowerSchedule
{
    /// <summary>
    /// 计划 Id
    /// </summary>
    public TowerScheduleId Id { get; set; }

    /// <summary>
    /// 层 Id 表
    /// </summary>
    public List<TowerFloorId> FloorIds { get; set; } = default!;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTimeOffset Open { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTimeOffset Close { get; set; }

    /// <summary>
    /// 渊月祝福
    /// </summary>
    public string BuffName { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public List<string> Descriptions { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;
}