// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Navigation;

namespace Snap.Hutao.ViewModel.Game;

internal sealed class LaunchGameExtraData : NavigationExtraData<string>
{
    private LaunchGameExtraData(string uid)
        : base(uid)
    {
    }

    public static INavigationCompletionSource CreateForUid(string? uid)
    {
        return uid is null ? Default : new LaunchGameExtraData(uid);
    }
}