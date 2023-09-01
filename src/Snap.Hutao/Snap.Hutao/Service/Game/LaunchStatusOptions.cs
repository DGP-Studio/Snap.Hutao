// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Service.Game;

[Injection(InjectAs.Singleton)]
internal sealed class LaunchStatusOptions : ObservableObject
{
    private LaunchStatus? launchStatus;

    public LaunchStatus? LaunchStatus { get => launchStatus; set => SetProperty(ref launchStatus, value); }
}