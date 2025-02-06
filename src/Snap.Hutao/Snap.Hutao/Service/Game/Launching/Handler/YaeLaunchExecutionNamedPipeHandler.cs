// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle.InterProcess.Yae;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.User32;

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
            return;
        }

        if (!context.Options.IsIslandEnabled)
        {
            context.Logger.LogInformation("Island is not enabled");
            context.Result.Kind = LaunchExecutionResultKind.EmbeddedYaeIslandNotEnabled;
            context.Result.ErrorMessage = SH.ServiceGameLaunchingHandlerEmbeddedYaeIslandNotEnabled;
            return;
        }

        context.Logger.LogInformation("Initializing Yae");
        InitializeYae(context.Process);

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

    private static unsafe void InitializeYae(Process gameProcess)
    {
        string dataFolderYaePath = Path.Combine(HutaoRuntime.DataFolder, "YaeLib.dll");
        InstalledLocation.CopyFileFromApplicationUri("ms-appx:///YaeLib.dll", dataFolderYaePath);

        HANDLE hModule = default;
        try
        {
            hModule = NativeLibrary.Load(dataFolderYaePath);
            nint pYaeGetWindowHook = NativeLibrary.GetExport((nint)(hModule & ~0x3L), "YaeGetWindowHook");

            HOOKPROC hookProc = default;
            ((delegate* unmanaged[Stdcall]<HOOKPROC*, HRESULT>)pYaeGetWindowHook)(&hookProc);

            SpinWait.SpinUntil(() => gameProcess.MainWindowHandle is not 0);
            uint threadId = GetWindowThreadProcessId(gameProcess.MainWindowHandle, default);
            HHOOK hHook = SetWindowsHookExW(WINDOWS_HOOK_ID.WH_GETMESSAGE, hookProc, hModule, threadId);
            if (hHook.Value is 0)
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
                HutaoException.Throw("SetWindowsHookExW returned 'NULL' but no Error is presented");
            }

            if (!PostThreadMessageW(threadId, WM_NULL, default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
                HutaoException.Throw("PostThreadMessageW returned 'FALSE' but no Error is presented");
            }
        }
        finally
        {
            // NEVER UNHOOK: Will cause the dll unload in game process
            // UnhookWindowsHookEx(hHook);
            NativeLibrary.Free(hModule);
        }
    }
}