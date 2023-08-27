// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;

/// <summary>
/// 验证结果
/// </summary>
[HighQuality]
internal sealed class VerificationResult
{
    /// <summary>
    /// 极验流水号
    /// </summary>
    [JsonPropertyName("challenge")]
    public string? Challenge { get; set; }
}