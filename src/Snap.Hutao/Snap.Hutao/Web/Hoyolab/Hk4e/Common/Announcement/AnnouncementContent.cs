// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告内容
/// </summary>
public class AnnouncementContent
{
    /// <summary>
    /// 公告Id
    /// </summary>
    [JsonProperty("ann_id")]
    public int AnnId { get; set; }

    /// <summary>
    /// 公告标题
    /// </summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 副标题
    /// </summary>
    [JsonProperty("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>
    /// 横幅Url
    /// </summary>
    [JsonProperty("banner")]
    public string? Banner { get; set; }

    /// <summary>
    /// 内容字符串
    /// 可能包含了一些html格式
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    [JsonProperty("lang")]
    public string? Lang { get; set; }
}
