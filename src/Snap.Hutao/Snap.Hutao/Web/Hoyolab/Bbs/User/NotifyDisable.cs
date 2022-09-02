// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 通知提醒
/// </summary>
public class NotifyDisable
{
    /// <summary>
    /// 回复我
    /// </summary>
    [JsonPropertyName("reply")]
    public bool Reply { get; set; }

    /// <summary>
    /// 给我点赞
    /// </summary>
    [JsonPropertyName("upvote")]
    public bool Upvote { get; set; }

    /// <summary>
    /// 关注了我
    /// </summary>
    [JsonPropertyName("follow")]
    public bool Follow { get; set; }

    /// <summary>
    /// 系统通知
    /// </summary>
    [JsonPropertyName("system")]
    public bool System { get; set; }

    /// <summary>
    /// 私信通知
    /// </summary>
    [JsonPropertyName("chat")]
    public bool Chat { get; set; }
}
