// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Security.Principal;
using Snap.Hutao.Win32;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using WinRT;

[assembly: DisableRuntimeMarshalling]

namespace Snap.Hutao;

[SuppressMessage("", "SH001")]
public static partial class Bootstrap
{
    private const string LockName = "SNAP_HUTAO_BOOTSTRAP_LOCK";
    private static readonly ApplicationInitializationCallback AppInitializationCallback = InitializeApp;
    private static Mutex? mutex;

    internal static void UseNamedPipeRedirection()
    {
        Debug.Assert(mutex is not null);
        DisposableMarshal.DisposeAndClear(ref mutex);
    }

    [STAThread]
    private static void Main(string[] args)
    {
        if (Mutex.TryOpenExisting(LockName, out _))
        {
            return;
        }

        try
        {
            MutexSecurity mutexSecurity = new();
            mutexSecurity.AddAccessRule(new(SecurityIdentifiers.Everyone, MutexRights.FullControl, AccessControlType.Allow));
            mutex = MutexAcl.Create(true, LockName, out bool created, mutexSecurity);
            Debug.Assert(created);
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            return;
        }

        // Although we 'using' mutex there, the actual disposal is done in AppActivation
        // The using is just to ensure we dispose the mutex when the application exits
        using (mutex)
        {
            if (!OSPlatformSupported())
            {
                return;
            }

            Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00000000");
            Environment.SetEnvironmentVariable("DOTNET_SYSTEM_BUFFERS_SHAREDARRAYPOOL_MAXARRAYSPERPARTITION", "128");
            AppContext.SetData("MVVMTOOLKIT_ENABLE_INOTIFYPROPERTYCHANGING_SUPPORT", false);

            ComWrappersSupport.InitializeComWrappers();

            // By adding the using statement, we can dispose the injected services when closing
            using (ServiceProvider serviceProvider = DependencyInjection.Initialize())
            {
                Thread.CurrentThread.Name = "Snap Hutao Application Main Thread";

                // If you hit a COMException REGDB_E_CLASSNOTREG (0x80040154) during debugging
                // You can delete bin and obj folder and then rebuild.
                // In a Desktop app this runs a message pump internally,
                // and does not return until the application shuts down.
                Application.Start(AppInitializationCallback);
                XamlApplicationLifetime.Exited = true;
            }

            SentrySdk.Flush();
        }
    }

    private static void InitializeApp(ApplicationInitializationCallbackParams param)
    {
        Gen2GcCallback.Register(() =>
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateDebug("Gen2 GC triggered.", "Runtime"));
            return true;
        });

        IServiceProvider serviceProvider = Ioc.Default;

        _ = serviceProvider.GetRequiredService<ITaskContext>();
        _ = serviceProvider.GetRequiredService<App>();
    }

    private static bool OSPlatformSupported()
    {
        if (!HutaoNative.Instance.IsCurrentWindowsVersionSupported())
        {
            const string Message = """
                Snap Hutao 无法在版本低于 10.0.19045.5371 的 Windows 上运行，请更新系统。
                Snap Hutao cannot run on Windows versions earlier than 10.0.19045.5371. Please update your system.
                """;
            HutaoNative.Instance.ShowErrorMessage("Warning | 警告", Message);
            return false;
        }

        return true;
    }
}