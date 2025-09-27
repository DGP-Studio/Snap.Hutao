// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.Game;

namespace Snap.Hutao.Service.Game.Launching.Context;

internal sealed class LaunchExecutionInvocationContext
{
    public required IViewModelSupportLaunchExecution ViewModel { get; init; }

    public required IServiceProvider ServiceProvider { get; init; }

    public required LaunchOptions LaunchOptions { get; init; }

    public required GameIdentity Identity { get; init; }
}