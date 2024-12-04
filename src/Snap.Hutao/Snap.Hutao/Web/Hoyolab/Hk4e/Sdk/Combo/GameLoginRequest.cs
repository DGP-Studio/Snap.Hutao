// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Sdk.Combo;

internal sealed class GameLoginRequest
{
    [JsonPropertyName("app_id")]
    public int AppId { get; set; }

    [JsonPropertyName("device")]
    public string Device { get; set; } = default!;

    [JsonPropertyName("ticket")]
    public string? Ticket { get; set; }

    public static GameLoginRequest Create(int appId, string device, string? ticket = null)
    {
        return new()
        {
            AppId = appId,
            Device = device,
            Ticket = ticket,
        };
    }
}
