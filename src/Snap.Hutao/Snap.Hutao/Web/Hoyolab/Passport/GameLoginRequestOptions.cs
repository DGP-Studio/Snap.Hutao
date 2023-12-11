// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 扫码登录请求配置
/// </summary>
[HighQuality]
internal sealed class GameLoginRequestOptions
{
    [JsonPropertyName("app_id")]
    public int AppId { get; set; }

    [JsonPropertyName("device")]
    public string Device { get; set; } = default!;

    public static GameLoginRequestOptions Create(int appId, string device)
    {
        return new GameLoginRequestOptions
        {
            AppId = appId,
            Device = device,
        };
    }
}
