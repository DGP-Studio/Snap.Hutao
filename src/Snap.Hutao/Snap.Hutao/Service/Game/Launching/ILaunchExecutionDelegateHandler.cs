// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

internal delegate ValueTask<LaunchExecutionContext> LaunchExecutionDelegate();

internal interface ILaunchExecutionDelegateHandler
{
    ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next);
}