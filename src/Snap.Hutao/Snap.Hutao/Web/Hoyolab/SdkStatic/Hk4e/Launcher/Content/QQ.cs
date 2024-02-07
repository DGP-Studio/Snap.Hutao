// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Content;

internal sealed class QQ
{
    [JsonPropertyName("qq_id")]
    public string QQId { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("number")]
    public string Number { get; set; } = default!;

    [JsonPropertyName("code")]
    public string Code { get; set; } = default!;
}