// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Binding;

namespace Snap.Hutao.Service.User;

internal interface IUserInitializationService
{
    ValueTask<ViewModel.User.User?> CreateUserFromInputCookieOrDefaultAsync(InputCookie inputCookie, CancellationToken token = default(CancellationToken));

    ValueTask<ViewModel.User.User> ResumeUserAsync(Model.Entity.User inner, CancellationToken token = default(CancellationToken));
}