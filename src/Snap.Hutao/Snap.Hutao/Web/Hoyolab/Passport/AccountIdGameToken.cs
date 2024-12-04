// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class AccountIdGameToken
{
    [JsonPropertyName("account_id")]
    public int AccountId { get; set; }

    [JsonPropertyName("game_token")]
    public string GameToken { get; set; } = default!;
}
