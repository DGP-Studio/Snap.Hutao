// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 扫码登录请求结果
/// </summary>
[HighQuality]
internal sealed class GameLoginRequestResult
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
}
