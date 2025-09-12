// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureSchemeHandler : AbstractLaunchExecutionHandler
{
    public override ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (context.TargetScheme is null)
        {
            return ValueTask.FromException(HutaoException.InvalidOperation(SH.ViewModelLaunchGameSchemeNotSelected));
        }

        return ValueTask.CompletedTask;
    }
}