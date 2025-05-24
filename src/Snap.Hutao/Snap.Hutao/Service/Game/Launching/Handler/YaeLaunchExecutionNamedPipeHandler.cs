// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle.InterProcess.Yae;
using Snap.Hutao.Service.Game.Island;
using System.IO;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class YaeLaunchExecutionNamedPipeHandler : ILaunchExecutionDelegateHandler
{
    private readonly YaeDataArrayReceiver receiver;

    public YaeLaunchExecutionNamedPipeHandler(YaeDataArrayReceiver receiver)
    {
        this.receiver = receiver;
    }

    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!HutaoRuntime.IsProcessElevated)
        {
            context.Logger.LogInformation("Process is not elevated");
            context.Result.Kind = LaunchExecutionResultKind.EmbeddedYaeClientNotElevated;
            context.Result.ErrorMessage = SH.ServiceGameLaunchingHandlerEmbeddedYaeClientNotElevated;
            context.Process.Kill();
            return;
        }

        if (!context.Options.IsIslandEnabled)
        {
            context.Logger.LogInformation("Island is not enabled");
            context.Result.Kind = LaunchExecutionResultKind.EmbeddedYaeIslandNotEnabled;
            context.Result.ErrorMessage = SH.ServiceGameLaunchingHandlerEmbeddedYaeIslandNotEnabled;
            context.Process.Kill();
            return;
        }

        context.Logger.LogInformation("Initializing Yae");
        string dataFolderYaePath = Path.Combine(HutaoRuntime.DataFolder, "YaeLib.dll");
        InstalledLocation.CopyFileFromApplicationUri("ms-appx:///YaeLib.dll", dataFolderYaePath);
        DllInjectionUtilities.InjectUsingWindowsHook(dataFolderYaePath, "YaeGetWindowHook", context.Process.Id);

        try
        {
#pragma warning disable CA2007
            await using (YaeNamedPipeServer server = new(context.ServiceProvider, context.Process))
#pragma warning restore CA2007
            {
                receiver.Array = await server.GetDataArrayAsync().ConfigureAwait(false);
            }

            await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            context.Result.Kind = LaunchExecutionResultKind.EmbeddedYaeNamedPipeError;
            context.Result.ErrorMessage = ex.Message;
            context.Process.Kill();
        }
    }
}