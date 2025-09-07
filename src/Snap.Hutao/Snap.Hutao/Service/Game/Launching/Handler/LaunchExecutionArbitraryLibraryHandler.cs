// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Game.Island;
using System.IO;
using Windows.Storage;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionArbitraryLibraryHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (HutaoRuntime.IsProcessElevated && context.Options.IsIslandEnabled.Value)
        {
            ApplicationDataCompositeValue value = LocalSetting.Get<ApplicationDataCompositeValue>(SettingKeys.LaunchExecutionArbitraryLibrary, []);

            try
            {
                foreach ((string md5, object path) in value)
                {
                    string pathString = path.ToString() ?? string.Empty;
                    if (File.Exists(pathString))
                    {
                        DllInjectionUtilities.InjectUsingRemoteThread(pathString, context.Process.Id);
                    }
                }

                context.Process.ResumeMainThread();
            }
            catch (Exception ex)
            {
                context.Result.Kind = LaunchExecutionResultKind.GameIslandOperationFailed;
                context.Result.ErrorMessage = ex.Message;
                context.Process.Kill();
            }
        }

        await next().ConfigureAwait(false);
    }
}