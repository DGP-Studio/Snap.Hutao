// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Account;

internal sealed class GameTokenWrapper
{
    [JsonPropertyName("account_id")]
    public int Stuid { get; set; } = default!;

    [JsonPropertyName("game_token")]
    public string GameToken { get; set; } = default!;
}
