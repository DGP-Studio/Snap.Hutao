// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.System.Com;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// App 实例拓展
/// </summary>
internal static class AppInstanceExtension
{
    /// <summary>
    /// 同步非阻塞重定向
    /// </summary>
    /// <param name="appInstance">app实例</param>
    /// <param name="args">参数</param>
    [SuppressMessage("", "VSTHRD110")]
    public static void RedirectActivationTo(this AppInstance appInstance, AppActivationArguments args)
    {
        HANDLE redirectEventHandle = UnsafeCreateEvent();
        Task.Run(async () =>
        {
            await appInstance.RedirectActivationToAsync(args);
            SetEvent(redirectEventHandle);
        });

        ReadOnlySpan<HANDLE> handles = new(in redirectEventHandle);
        CoWaitForMultipleObjects((uint)CWMO_FLAGS.CWMO_DEFAULT, INFINITE, handles, out uint _);
    }

    private static unsafe HANDLE UnsafeCreateEvent()
    {
        return CreateEvent(default(SECURITY_ATTRIBUTES*), true, false, null);
    }
}