// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 极验
/// </summary>
public class Aigis
{
    /// <summary>
    /// 极验会话Id
    /// </summary>
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = default!;

    /// <summary>
    /// 验证类型 1
    /// </summary>
    [JsonPropertyName("mmt_type")]
    public int MmtType { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    [JsonPropertyName("data")]
    public string Data { get; set; } = default!;
}