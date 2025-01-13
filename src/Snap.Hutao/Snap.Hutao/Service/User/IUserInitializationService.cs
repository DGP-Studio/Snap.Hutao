// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.User;

internal interface IUserInitializationService
{
    ValueTask<ViewModel.User.User?> CreateUserFromInputCookieOrDefaultAsync(InputCookie inputCookie, CancellationToken token = default);

    ValueTask<ViewModel.User.User> ResumeUserAsync(Model.Entity.User entity, CancellationToken token = default);

    ValueTask<ViewModel.User.User> ResumeUserAsync(ViewModel.User.User user, CancellationToken token = default);
}