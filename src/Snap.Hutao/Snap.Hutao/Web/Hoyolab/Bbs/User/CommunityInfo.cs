// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class CommunityInfo
{
    [JsonPropertyName("is_realname")]
    public bool IsRealname { get; set; }

    [JsonPropertyName("agree_status")]
    public bool AgreeStatus { get; set; }

    [JsonPropertyName("silent_end_time")]
    public long SilentEndTime { get; set; }

    [JsonPropertyName("forbid_end_time")]
    public long ForbidEndTime { get; set; }

    [JsonPropertyName("info_upd_time")]
    public long InfoUpdatedTime { get; set; }

    [JsonPropertyName("privacy_invisible")]
    public PrivacyInvisible PrivacyInvisible { get; set; } = default!;

    [JsonPropertyName("notify_disable")]
    public NotifyDisable NotifyDisable { get; set; } = default!;

    [JsonPropertyName("has_initialized")]
    public bool HasInitialized { get; set; }

    [JsonPropertyName("user_func_status")]
    public UserFuncStatus UserFuncStatus { get; set; } = default!;

    [JsonPropertyName("forum_silent_info")]
    public List<JsonElement> ForumSilentInfo { get; set; } = default!;

    [JsonPropertyName("last_login_ip")]
    public string LastLoginIp { get; set; } = default!;

    [JsonPropertyName("last_login_time")]
    public long LastLoginTime { get; set; } = default!;

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; } = default!;
}