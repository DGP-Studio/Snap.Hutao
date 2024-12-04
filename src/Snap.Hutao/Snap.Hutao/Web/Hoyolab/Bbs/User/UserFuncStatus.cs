// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class UserFuncStatus
{
    [JsonPropertyName("enable_history_view")]
    public bool EnableHistoryView { get; set; }

    [JsonPropertyName("enable_recommend")]
    public bool EnableRecommend { get; set; }

    [JsonPropertyName("enable_mention")]
    public bool EnableMention { get; set; }
}