// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 验证密钥
/// </summary>
[HighQuality]
internal sealed class GameAuthKey
{
    /// <summary>
    /// 验证密钥
    /// </summary>
    [JsonPropertyName("authkey")]
    public string AuthKey { get; set; } = default!;

    /// <summary>
    /// 验证密钥版本
    /// </summary>
    [JsonPropertyName("authkey_ver")]
    public int AuthKeyVersion { get; set; } = default!;

    /// <summary>
    /// 签名类型
    /// </summary>
    [JsonPropertyName("sign_type")]
    public int SignType { get; set; } = default!;
}
