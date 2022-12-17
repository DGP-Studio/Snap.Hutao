// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 祈愿卡池配置
/// </summary>
public class GachaEvent
{
    /// <summary>
    /// 卡池名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 版本
    /// </summary>
    public string Version { get; set; } = default!;

    /// <summary>
    /// 卡池图
    /// </summary>
    public Uri Banner { get; set; } = default!;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTimeOffset From { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTimeOffset To { get; set; }

    /// <summary>
    /// 卡池类型
    /// </summary>
    public GachaConfigType Type { get; set; }

    /// <summary>
    /// 五星列表
    /// </summary>
    public List<string> UpOrangeList { get; set; } = default!;

    /// <summary>
    /// 四星列表
    /// </summary>
    public List<string> UpPurpleList { get; set; } = default!;
}