// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class GameReception
{
    [JsonPropertyName("position")]
    public required int Position { get; init; }

    [JsonPropertyName("data")]
    public required GameReceptionData Data { get; init; }

    [JsonPropertyName("expose_ticket")]
    public required string ExposeTicket { get; init; }
}