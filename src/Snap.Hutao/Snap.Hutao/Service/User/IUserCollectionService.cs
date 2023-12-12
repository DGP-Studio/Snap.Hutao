// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.ViewModel.User.User;

namespace Snap.Hutao.Service.User;

internal interface IUserCollectionService
{
    BindingUser? CurrentUser { get; set; }

    ValueTask<ObservableCollection<UserAndUid>> GetUserAndUidCollectionAsync();

    ValueTask<ObservableCollection<BindingUser>> GetUserCollectionAsync();

    UserGameRole? GetUserGameRoleByUid(string uid);

    ValueTask RemoveUserAsync(BindingUser user);

    ValueTask<ValueResult<UserOptionResult, string>> TryCreateAndAddUserFromCookieAsync(Cookie cookie, bool isOversea);

    bool TryGetUserByMid(string mid, [NotNullWhen(true)] out BindingUser? user);
}