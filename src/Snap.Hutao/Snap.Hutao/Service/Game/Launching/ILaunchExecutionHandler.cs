// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

internal interface ILaunchExecutionHandler
{
    ValueTask BeforeAsync(BeforeLaunchExecutionContext context);

    ValueTask ExecuteAsync(LaunchExecutionContext context);

    ValueTask AfterAsync(AfterLaunchExecutionContext context);
}