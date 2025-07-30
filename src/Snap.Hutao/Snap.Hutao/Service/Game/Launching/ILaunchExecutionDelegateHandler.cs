// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

internal delegate ValueTask LaunchExecutionDelegate();

internal delegate ValueTask<bool> BeforeExecutionDelegate();

internal interface ILaunchExecutionDelegateHandler
{
    ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next);

    ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next);
}