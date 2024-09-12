﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

internal interface IUserService
{
    ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUsersAsync();

    ValueTask<ValueResult<UserOptionResult, string>> ProcessInputCookieAsync(InputCookie inputCookie);

    ValueTask<bool> RefreshCookieTokenAsync(EntityUser user);

    ValueTask RemoveUserAsync(BindingUser user);

    ValueTask RefreshProfilePictureAsync(UserGameRole userGameRole);
}