// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

internal interface IUserCollectionService
{
    ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUsersAsync();

    ValueTask RemoveUserAsync(BindingUser user);

    ValueTask<ValueResult<UserOptionResultKind, string?>> TryCreateAndAddUserFromInputCookieAsync(InputCookie inputCookie);
}