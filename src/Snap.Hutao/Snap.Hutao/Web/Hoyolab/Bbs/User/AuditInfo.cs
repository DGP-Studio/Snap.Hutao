// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 审核信息
/// </summary>
public class AuditInfo
{
    /// <summary>
    /// 昵称是否正在审核
    /// </summary>
    [JsonPropertyName("is_nickname_in_audit")]
    public bool IsNicknameInAudit { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    /// <summary>
    /// 简介是否正在审核
    /// </summary>
    [JsonPropertyName("is_introduce_in_audit")]
    public bool IsIntroduceInAudit { get; set; }

    /// <summary>
    /// 简介
    /// </summary>
    [JsonPropertyName("introduce")]
    public string Introduce { get; set; } = default!;

    /// <summary>
    /// 昵称状态
    /// </summary>
    [JsonPropertyName("nickname_status")]
    public int NicknameStatus { get; set; }
}