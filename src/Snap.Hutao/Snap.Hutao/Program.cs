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

    [LibraryImport("Microsoft.ui.xaml.dll")]
    private static partial void XamlCheckProcessRequirements();

    [STAThread]
    private static void Main(string[] args)
    {
        System.Diagnostics.Debug.WriteLine($"[Arguments]:{args}");

        XamlCheckProcessRequirements();
        ComWrappersSupport.InitializeComWrappers();

        // By adding the using statement, we can dispose the injected services when we closing
        using (ServiceProvider serviceProvider = DependencyInjection.Initialize())
        {
            serviceProvider.InitializeCulture();

            // In a Desktop app this runs a message pump internally,
            // and does not return until the application shuts down.
            Application.Start(AppInitializationCallback);
            Control.ScopedPage.DisposePreviousScope();
        }
    }

    private static void InitializeApp(ApplicationInitializationCallbackParams param)
    {
        IServiceProvider serviceProvider = Ioc.Default;

        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        _ = serviceProvider.GetRequiredService<App>();
    }
}