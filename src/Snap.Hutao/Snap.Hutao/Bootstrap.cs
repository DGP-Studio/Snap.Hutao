// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Security.Principal;
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
        mutex?.Dispose();
    }

    [ModuleInitializer]
    internal static void InitializeModule()
    {
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

    [LibraryImport("Microsoft.ui.xaml.dll")]
    private static partial void XamlCheckProcessRequirements();

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

            Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00000000");
            Environment.SetEnvironmentVariable("DOTNET_SYSTEM_BUFFERS_SHAREDARRAYPOOL_MAXARRAYSPERPARTITION", "128");

            Debug.WriteLine($"[Arguments]:[{args.ToString(',')}]");
            XamlCheckProcessRequirements();
            ComWrappersSupport.InitializeComWrappers();

            // Sentry requires version information, which depends on COM initialization
            using (SentryBootstrap.Initialize())
            {
                // By adding the using statement, we can dispose the injected services when closing
                using (DependencyInjection.Initialize())
                {
                    // In a Desktop app this runs a message pump internally,
                    // and does not return until the application shuts down.
                    Thread.CurrentThread.Name = "Snap Hutao Application Main Thread";

                    // If you hit a COMException REGDB_E_CLASSNOTREG (0x80040154) during debugging
                    // You can delete bin and obj folder and then rebuild.
                    Application.Start(AppInitializationCallback);
                }
            }
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            // Ignored
        }
        finally
        {
            mutex?.Dispose();
        }
    }

    private static void InitializeApp(ApplicationInitializationCallbackParams param)
    {
        IServiceProvider serviceProvider = Ioc.Default;

        _ = serviceProvider.GetRequiredService<ITaskContext>();
        XamlApplicationLifetime.DispatcherQueueInitialized = true;
        _ = serviceProvider.GetRequiredService<App>();
    }
}