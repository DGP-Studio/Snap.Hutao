// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Win32;
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
    private const string WindowsVersionPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
    private const string MajorKey = "CurrentMajorVersionNumber";
    private const string MinorKey = "CurrentMinorVersionNumber";
    private const string BuildKey = "CurrentBuildNumber";
    private const string RevisionKey = "UBR";

    private readonly Exception exception;

    public ExceptionWindow(Exception exception)
    {
        // Message pump will die if we introduce XamlWindowController
        InitializeComponent();
        this.exception = exception;

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
        }

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
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(WindowsVersionPath);
            ArgumentNullException.ThrowIfNull(key);
            object? major = key.GetValue(MajorKey);
            object? minor = key.GetValue(MinorKey);
            object? build = key.GetValue(BuildKey);
            object? revision = key.GetValue(RevisionKey);
            string windowsVersion = $"Windows {major}.{minor}.{build}.{revision}";

            return $"""
                {windowsVersion}
                System Architecture: {RuntimeInformation.OSArchitecture}
                Process Architecture: {RuntimeInformation.ProcessArchitecture}
                Framework: {RuntimeInformation.FrameworkDescription}

                {ExceptionFormat.Format(exception)}
                """;
        }
    }

    public static void Show(Exception ex)
    {
        ExceptionWindow window = new(ex);
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