// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Service.Notification;
using Windows.System;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionBetterGenshinImpactAutomationHandler : AbstractLaunchExecutionHandler
{
    public override async ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        if (context.Process.IsRunning() && context.LaunchOptions.UsingBetterGenshinImpactAutomation.Value)
        {
            await LaunchBetterGenshinImpactAsync(context).ConfigureAwait(false);
        }
    }

    private static async ValueTask LaunchBetterGenshinImpactAsync(LaunchExecutionContext context)
    {
        Uri betterGenshinImpactUri = "bettergi://start".ToUri();
        if (await Launcher.QueryUriSupportAsync(betterGenshinImpactUri, LaunchQuerySupportType.Uri) is not LaunchQuerySupportStatus.Available)
        {
            context.Messenger.Send(InfoBarMessage.Warning(SH.ServiceGameLaunchExecutionBetterGenshinImpactUrlProtocolNotRegistered));
            return;
        }

        try
        {
            SpinWaitPolyfill.SpinUntil(context.Process, static process => process.MainWindowHandle.Value is not 0);
        }
        catch (Exception ex)
        {
            context.Messenger.Send(InfoBarMessage.Error(SH.ServiceGameLaunchExecutionBetterGenshinImpactWaitGameMainWindowException, ex));
            return;
        }

        context.Messenger.Send(InfoBarMessage.Information(SH.ServiceGameLaunchExecutionBetterGenshinImpactStarted));
        await Launcher.LaunchUriAsync(betterGenshinImpactUri);
    }
}