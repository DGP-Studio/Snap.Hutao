// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// ?
/// </summary>
public class UserFuncStatus
{
    /// <summary>
    /// ?
    /// </summary>
    [JsonPropertyName("enable_history_view")]
    public bool EnableHistoryView { get; set; }

    /// <summary>
    /// ?
    /// </summary>
    [JsonPropertyName("enable_recommend")]
    public bool EnableRecommend { get; set; }

    /// <summary>
    /// ?
    /// </summary>
    [JsonPropertyName("enable_mention")]
    public bool EnableMention { get; set; }
}