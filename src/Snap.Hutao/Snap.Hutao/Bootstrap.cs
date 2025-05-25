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

// Visible to test project.
[assembly: InternalsVisibleTo("Snap.Hutao.Test")]
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

    [ModuleInitializer]
    internal static void InitializeModule()
    {
        // UndockedRegFreeWinRT
        // Set base directory env var for PublishSingleFile support (referenced by SxS redirection)
        Environment.SetEnvironmentVariable("MICROSOFT_WINDOWSAPPRUNTIME_BASE_DIRECTORY", AppContext.BaseDirectory);

        // No error handling needed as the target function does nothing (just {return S_OK}).
        // It's the act of calling the function causing the DllImport to load the DLL that
        // matters. This provides the moral equivalent of a native DLL's Import Address
        // Table (IAT) have an entry that's resolved when this module is loaded.
        _ = WindowsAppRuntimeEnsureIsLoaded();
    }

    [LibraryImport("Microsoft.WindowsAppRuntime.dll", EntryPoint = "WindowsAppRuntime_EnsureIsLoaded")]
    private static partial int WindowsAppRuntimeEnsureIsLoaded();

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

            ComWrappersSupport.InitializeComWrappers();

            // By adding the using statement, we can dispose the injected services when closing
            using (Core.DependencyInjection.Implementation.ServiceProvider serviceProvider = DependencyInjection.Initialize())
            {
                Thread.CurrentThread.Name = "Snap Hutao Application Main Thread";

                // If you hit a COMException REGDB_E_CLASSNOTREG (0x80040154) during debugging
                // You can delete bin and obj folder and then rebuild.
                // In a Desktop app this runs a message pump internally,
                // and does not return until the application shuts down.
                Application.Start(AppInitializationCallback);
                XamlApplicationLifetime.Exited = true;
            }
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
                Snap Hutao 无法在非 Windows 10 或 Windows 11 上运行，请更新系统。
                Snap Hutao cannot run on non-Windows 10 or Windows 11. Please update your system.
                """;
            HutaoNative.Instance.ShowErrorMessage("Warning | 警告", Message);
            return false;
        }

        return true;
    }
}