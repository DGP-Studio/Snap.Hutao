// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hutao.Geetest;

namespace Snap.Hutao.Service.Geetest;

internal interface IGeetestService
{
    ValueTask<GeetestData?> TryVerifyAsync(string gt, string challenge, CancellationToken token = default);

    ValueTask<string?> TryValidateXrpcChallengeAsync(Model.Entity.User user, CardVerifiationHeaders headers, CancellationToken token = default);

    ValueTask<bool> TryResolveAigisAsync(IAigisProvider provider, string? rawSession, bool isOversea, CancellationToken token = default);
}