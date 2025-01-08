// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Service.Game;

[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchStatusOptions : ObservableObject
{
    [ObservableProperty]
    public partial LaunchStatus? LaunchStatus { get; set; }
}