// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching.Context;

internal sealed class AfterLaunchExecutionContext
{
    public required IServiceProvider ServiceProvider { get; init; }

    public required ITaskContext TaskContext { get; init; }
}