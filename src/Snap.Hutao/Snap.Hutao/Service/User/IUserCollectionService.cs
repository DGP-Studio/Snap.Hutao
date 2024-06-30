// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

internal interface IUserCollectionService
{
    ValueTask<ObservableCollection<UserAndUid>> GetUserAndUidCollectionAsync();

    ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUserCollectionAsync();

    UserGameRole? GetUserGameRoleByUid(string uid);

    ValueTask RemoveUserAsync(BindingUser user);

    ValueTask<ValueResult<UserOptionResult, string>> TryCreateAndAddUserFromInputCookieAsync(InputCookie inputCookie);

    bool TryGetUserByMid(string mid, [NotNullWhen(true)] out BindingUser? user);
}