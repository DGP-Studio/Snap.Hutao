// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal interface IHoyoPlayPassportClient
{
    ValueTask<Response<AuthTicketWrapper>> CreateAuthTicketAsync(User user, CancellationToken token = default);

    ValueTask<Response<QrLogin>> CreateQrLoginAsync(CancellationToken token = default);

    ValueTask<Response<QrLoginResult>> QueryQrLoginStatusAsync(string ticket, CancellationToken token = default);

    ValueTask<(string? Aigis, string? Risk, Response<LoginResult> Response)> LoginByPasswordAsync(IPassportPasswordProvider provider, CancellationToken token = default);

    ValueTask<(string? Aigis, string? Risk, Response<LoginResult> Response)> LoginByPasswordAsync(string account, string password, string? aigis, string? verify, CancellationToken token = default);

    ValueTask<(string? Risk, Response<LoginResult> Response)> LoginByThirdPartyAsync(ThirdPartyToken thirdPartyToken, CancellationToken token = default);
}