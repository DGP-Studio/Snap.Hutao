// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;

namespace Snap.Hutao.Service.Game;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class LaunchStatusOptions : ObservableObject
{
    [ObservableProperty]
    public partial UserGameRole? UserGameRole { get; set; }

    [ObservableProperty]
    public partial LaunchStatus? LaunchStatus { get; set; }
}