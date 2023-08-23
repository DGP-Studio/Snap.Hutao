// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;

/// <summary>
/// 验证注册
/// </summary>
[HighQuality]
internal sealed class VerificationRegistration
{
    /// <summary>
    /// 极验流水号
    /// </summary>
    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = default!;

    /// <summary>
    /// 极验Id
    /// </summary>
    [JsonPropertyName("gt")]
    public string Gt { get; set; } = default!;

    /// <summary>
    /// 新验证
    /// </summary>
    public int NewCaptcha { get; set; }

    /// <summary>
    /// 是否成功注册
    /// </summary>
    public int Success { get; set; }
}