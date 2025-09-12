// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Game.Island;
using Snap.Hutao.Service.Notification;
using System.IO;
using Windows.Storage;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionArbitraryLibraryHandler : AbstractLaunchExecutionHandler
{
    public override ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        if (HutaoRuntime.IsProcessElevated && context.Options.IsIslandEnabled.Value)
        {
            return Execute(context);
        }

        return ValueTask.CompletedTask;
    }

    private static ValueTask Execute(LaunchExecutionContext context)
    {
        ApplicationDataCompositeValue value = LocalSetting.Get<ApplicationDataCompositeValue>(SettingKeys.LaunchExecutionArbitraryLibrary, []);

        foreach ((_, object path) in value)
        {
            string pathString = path.ToString() ?? string.Empty;
            try
            {
                // File.Exists handles null/empty/whitespace first
                if (File.Exists(pathString) && string.Equals(Path.GetExtension(pathString), ".dll", StringComparison.OrdinalIgnoreCase))
                {
                    DllInjectionUtilities.InjectUsingRemoteThread(pathString, context.Process.Id);
                }
            }
            catch (Exception ex)
            {
                context.Messenger.Send(InfoBarMessage.Error(SH.FormatServiceGameLaunchExecutionArbitraryLibraryInjectionException(pathString), ex));
                context.Process.Kill();
                return ValueTask.FromException(ex);
            }
        }

        context.Process.ResumeMainThread();
        return ValueTask.CompletedTask;
    }
}