// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// Visible to test project.
[assembly: InternalsVisibleTo("Snap.Hutao.Test")]

namespace Snap.Hutao;

/// <summary>
/// Program class
/// </summary>
[SuppressMessage("", "SH001")]
public static partial class Program
{
    private static readonly ApplicationInitializationCallback AppInitializationCallback = InitializeApp;

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
        System.Diagnostics.Debug.WriteLine($"[Arguments]:[{args.ToString(',')}]");

        XamlCheckProcessRequirements();
        ComWrappersSupport.InitializeComWrappers();

        // By adding the using statement, we can dispose the injected services when we closing
        using (ServiceProvider serviceProvider = DependencyInjection.Initialize())
        {
            // In a Desktop app this runs a message pump internally,
            // and does not return until the application shuts down.
            Application.Start(AppInitializationCallback);
        }
    }

    private static void InitializeApp(ApplicationInitializationCallbackParams param)
    {
        IServiceProvider serviceProvider = Ioc.Default;

        _ = serviceProvider.GetRequiredService<ITaskContext>();
        _ = serviceProvider.GetRequiredService<App>();
    }
}