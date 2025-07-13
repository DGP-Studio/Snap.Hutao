// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class TokenSet
{
    public required string AccessToken { get; set; }

    public required string RefreshToken { get; set; }

    public required int ExpiresIn { get; set; }
}