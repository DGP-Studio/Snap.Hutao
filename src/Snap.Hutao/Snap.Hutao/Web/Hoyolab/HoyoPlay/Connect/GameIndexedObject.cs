// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;

internal abstract class GameIndexedObject
{
    [JsonPropertyName("game")]
    public Game Game { get; set; } = default!;
}
