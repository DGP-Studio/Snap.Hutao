// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.Win32;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Core.LifeCycle;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

internal sealed partial class ExceptionWindow : Microsoft.UI.Xaml.Window
{
    private readonly string message;

    public ExceptionWindow(string message)
    {
        // Message pump will die if we introduce XamlWindowController
        InitializeComponent();
        this.message = message;

        AppWindow.Title = "Snap Hutao Exception Report";

        AppWindowTitleBar titleBar = AppWindow.TitleBar;
        titleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        titleBar.ExtendsContentIntoTitleBar = true;

        UpdateDragRectangles();
        DragableGrid.SizeChanged += (_, _) => UpdateDragRectangles();

        SizeInt32 size = new(800, 600);
        AppWindow.Resize(size.Scale(this.GetRasterizationScale()));

        Ioc.Default.GetRequiredService<ICurrentXamlWindowReference>().Window?.Close();
    }

    public string FormattedException
    {
        get
        {
            using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                string windowsVersion;
                if (key is not null)
                {
                    object? major = key.GetValue("CurrentMajorVersionNumber");
                    object? minor = key.GetValue("CurrentMinorVersionNumber");
                    object? build = key.GetValue("CurrentBuildNumber");
                    object? revision = key.GetValue("UBR");
                    windowsVersion = $"Windows {major}.{minor}.{build}.{revision}";
                }
                else
                {
                    windowsVersion = "Windows Version Unknown";
                }

                return $"""
                    Snap Hutao {HutaoRuntime.Version}
                    {windowsVersion}
                    System Architecture: {RuntimeInformation.OSArchitecture}
                    Process Architecture: {RuntimeInformation.ProcessArchitecture}
                    Framework: {RuntimeInformation.FrameworkDescription}

                    {message}
                    """;
            }
        }
    }

    public static void Show(string exMessage)
    {
        ExceptionWindow window = new(exMessage);
        window.Activate();
    }

    [Command("CloseCommand")]
    private static void CloseWindow()
    {
        Process.GetCurrentProcess().Kill();
    }

    private void UpdateDragRectangles()
    {
        if (DragableGrid.IsLoaded)
        {
            Point position = DragableGrid.TransformToVisual(Content).TransformPoint(default);
            RectInt32 dragRect = RectInt32Convert.RectInt32(position, DragableGrid.ActualSize).Scale(this.GetRasterizationScale());
            this.GetInputNonClientPointerSource().SetRegionRects(NonClientRegionKind.Caption, [dragRect]);
        }
    }
}