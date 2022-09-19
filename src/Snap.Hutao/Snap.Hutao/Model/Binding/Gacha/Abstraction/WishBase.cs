// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.Gacha.Abstraction;

/// <summary>
/// 祈愿基类
/// </summary>
public abstract class WishBase
{
    /// <summary>
    /// 卡池名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 统计开始时间
    /// </summary>
    public DateTimeOffset From { get; set; }

    /// <summary>
    /// 统计结束时间
    /// </summary>
    public DateTimeOffset To { get; set; }

    /// <summary>
    /// 统计开始时间
    /// </summary>
    public string FromFormatted
    {
        get => $"{From:yyyy.MM.dd}";
    }

    /// <summary>
    /// 统计开始时间
    /// </summary>
    public string ToFormatted
    {
        get => $"{To:yyyy.MM.dd}";
    }

    /// <summary>
    /// 总数
    /// </summary>
    public int TotalCount { get; set; }
}