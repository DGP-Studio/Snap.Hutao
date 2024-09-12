﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.Ole32;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// App 实例拓展
/// </summary>
[HighQuality]
internal static class AppInstanceExtension
{
    private static readonly WaitCallback RunActionWaitCallback = RunAction;

    // Hold the reference here to prevent memory corruption.
    private static HANDLE redirectEventHandle;

    public static unsafe void RedirectActivationTo(this AppInstance appInstance, AppActivationArguments args)
    {
        try
        {
            redirectEventHandle = CreateEventW(default, true, false, default);

            // use ThreadPool.UnsafeQueueUserWorkItem to cancel stacktrace
            // like ExecutionContext.SuppressFlow
            ThreadPool.UnsafeQueueUserWorkItem(RunActionWaitCallback, () =>
            {
                appInstance.RedirectActivationToAsync(args).AsTask().Wait();
                SetEvent(redirectEventHandle);
            });

            CoWaitForMultipleObjects(CWMO_FLAGS.CWMO_DEFAULT, INFINITE, [redirectEventHandle], out uint _);
        }
        finally
        {
            CloseHandle(redirectEventHandle);
        }
    }

    [SuppressMessage("", "SH007")]
    private static void RunAction(object? state)
    {
        ((Action)state!)();
    }
}