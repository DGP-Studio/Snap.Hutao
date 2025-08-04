// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle.InterProcess.Yae;
using Snap.Hutao.Service.Game.Island;
using Snap.Hutao.Service.Yae.Achievement;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.IO;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class YaeLaunchExecutionNamedPipeHandler : ILaunchExecutionDelegateHandler
{
    private readonly NativeConfiguration config;
    private readonly YaeDataArrayReceiver receiver;

    public YaeLaunchExecutionNamedPipeHandler(NativeConfiguration config, YaeDataArrayReceiver receiver)
    {
        this.config = config;
        this.receiver = receiver;
    }

    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
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

        try
        {
            DllInjectionUtilities.InjectUsingWindowsHook(dataFolderYaePath, "YaeGetWindowHook", context.Process.Id);
        }
        catch (Exception ex)
        {
            // Windows Defender Application Control
            if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_SYSTEM_INTEGRITY_POLICY_VIOLATION))
            {
                context.Result.Kind = LaunchExecutionResultKind.EmbeddedYaeNamedPipeError;
                context.Result.ErrorMessage = SH.ServiceGameLaunchingHandlerEmbeddedYaeErrorSystemIntegrityPolicyViolation;
                context.Process.Kill();
                return;
            }
        }

        try
        {
#pragma warning disable CA2007
            await using (YaeNamedPipeServer server = new(context.ServiceProvider, context.Process, config))
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