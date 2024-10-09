// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal interface IHoyoPlayPassportClient
{
    ValueTask<Response<AuthTicketWrapper>> CreateAuthTicketAsync(User user, CancellationToken token = default);
}