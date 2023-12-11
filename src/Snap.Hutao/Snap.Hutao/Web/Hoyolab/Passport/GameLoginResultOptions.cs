// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 扫码登录结果请求配置
/// </summary>
[HighQuality]
internal sealed class GameLoginResultOptions
{
    [JsonPropertyName("app_id")]
    public int AppId { get; set; }

    [JsonPropertyName("device")]
    public string Device { get; set; } = default!;

    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = default!;

    public static GameLoginResultOptions Create(int appId, string device, string ticket)
    {
        return new GameLoginResultOptions
        {
            AppId = appId,
            Device = device,
            Ticket = ticket,
        };
    }
}
