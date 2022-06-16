// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 社区信息
/// </summary>
public class CommunityInfo
{
    /// <summary>
    /// 是否实名
    /// </summary>
    [JsonPropertyName("is_realname")]
    public bool IsRealname { get; set; }

    /// <summary>
    /// 条款同意情况
    /// </summary>
    [JsonPropertyName("agree_status")]
    public bool AgreeStatus { get; set; }

    /// <summary>
    /// 禁言倒计时
    /// </summary>
    [JsonPropertyName("silent_end_time")]
    public long SilentEndTime { get; set; }

    /// <summary>
    /// 封禁倒计时
    /// </summary>
    [JsonPropertyName("forbid_end_time")]
    public long ForbidEndTime { get; set; }

    /// <summary>
    /// 信息更新时间戳
    /// </summary>
    [JsonPropertyName("info_upd_time")]
    public long InfoUpdTime { get; set; }

    /// <summary>
    /// 隐私设置
    /// </summary>
    [JsonPropertyName("privacy_invisible")]
    public PrivacyInvisible PrivacyInvisible { get; set; } = default!;

    /// <summary>
    /// 禁用的通知
    /// </summary>
    [JsonPropertyName("notify_disable")]
    public NotifyDisable NotifyDisable { get; set; } = default!;

    /// <summary>
    /// 是否初始化完成
    /// </summary>
    [JsonPropertyName("has_initialized")]
    public bool HasInitialized { get; set; }

    /// <summary>
    /// 用户功能状态
    /// </summary>
    [JsonPropertyName("user_func_status")]
    public UserFuncStatus UserFuncStatus { get; set; } = default!;

    /// <summary>
    /// ?
    /// </summary>
    [JsonPropertyName("forum_silent_info")]
    public List<JsonElement> ForumSilentInfo { get; set; } = default!;

    /// <summary>
    /// 最后登录的IP
    /// </summary>
    [JsonPropertyName("last_login_ip")]
    public string LastLoginIp { get; set; } = default!;

    /// <summary>
    /// 最后登录的时间
    /// </summary>
    [JsonPropertyName("last_login_time")]
    public long LastLoginTime { get; set; } = default!;

    /// <summary>
    /// 创建于
    /// </summary>
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; } = default!;
}
