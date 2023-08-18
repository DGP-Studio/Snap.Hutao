// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告内容
/// </summary>
[HighQuality]
internal class AnnouncementContent
{
    /// <summary>
    /// 公告Id
    /// </summary>
    [JsonPropertyName("ann_id")]
    public int AnnId { get; set; }

    /// <summary>
    /// 公告标题
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    /// <summary>
    /// 副标题
    /// </summary>
    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; } = default!;

    /// <summary>
    /// 横幅Url
    /// </summary>
    [JsonPropertyName("banner")]
    public string Banner { get; set; } = default!;

    /// <summary>
    /// 内容字符串
    /// 可能包含了一些html格式
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = default!;

    /// <summary>
    /// 语言
    /// </summary>
    [JsonPropertyName("lang")]
    public string Lang { get; set; } = default!;
}