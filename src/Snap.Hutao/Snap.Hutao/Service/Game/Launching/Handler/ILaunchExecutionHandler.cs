// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching.Context;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal interface ILaunchExecutionHandler
{
    ValueTask BeforeAsync(BeforeLaunchExecutionContext context);

    ValueTask ExecuteAsync(LaunchExecutionContext context);

    ValueTask AfterAsync(AfterLaunchExecutionContext context);
}