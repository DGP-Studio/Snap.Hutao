// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;

internal interface IGeetestCardVerifier
{
    ValueTask<string?> TryValidateXrpcChallengeAsync(User user, CancellationToken token);
}