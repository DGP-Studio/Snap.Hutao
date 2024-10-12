// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal interface IHoyoPlayPassportClient
{
    ValueTask<Response<AuthTicketWrapper>> CreateAuthTicketAsync(User user, CancellationToken token = default);

    ValueTask<(string? Aigis, Response<LoginResult> Response)> LoginByPassword(IPassportPasswordProvider provider, CancellationToken token = default);

    ValueTask<(string? Aigis, Response<LoginResult> Response)> LoginByPassword(string account, string password, string? aigis, CancellationToken token = default);

    ValueTask<Response<LoginResult>> LoginByThirdPartyAsync(ThirdPartyToken thirdPartyToken, CancellationToken token = default);
}