// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Account;
using Snap.Hutao.Service.Game.Launching.Context;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionWindowsHDRHandler : AbstractLaunchExecutionHandler
{
    public override ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (context.LaunchOptions.IsWindowsHDREnabled.Value)
        {
            RegistryInterop.SetWindowsHDR(context.TargetScheme.IsOversea);
        }

        return ValueTask.CompletedTask;
    }
}