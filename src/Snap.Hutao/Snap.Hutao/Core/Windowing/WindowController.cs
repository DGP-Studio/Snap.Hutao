// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service;
using System.IO;
using Windows.Graphics;
using Windows.UI;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

[SuppressMessage("", "CA1001")]
internal sealed class WindowController
{
    private readonly Window window;
    private readonly WindowOptions options;
    private readonly IServiceProvider serviceProvider;
    private readonly WindowSubclass subclass;

    public WindowController(Window window, in WindowOptions options, IServiceProvider serviceProvider)
    {
        this.window = window;
        this.options = options;
        this.serviceProvider = serviceProvider;

        subclass = new(window, options, serviceProvider);

        InitializeCore();
    }

    private static void TransformToCenterScreen(ref RectInt32 rect)
    {
        DisplayArea displayArea = DisplayArea.GetFromRect(rect, DisplayAreaFallback.Nearest);
        RectInt32 workAreaRect = displayArea.WorkArea;

        rect.Width = Math.Min(workAreaRect.Width, rect.Width);
        rect.Height = Math.Min(workAreaRect.Height, rect.Height);

        rect.X = workAreaRect.X + ((workAreaRect.Width - rect.Width) / 2);
        rect.Y = workAreaRect.Y + ((workAreaRect.Height - rect.Height) / 2);
    }

    private void InitializeCore()
    {
        RuntimeOptions hutaoOptions = serviceProvider.GetRequiredService<RuntimeOptions>();

        window.AppWindow.Title = SH.AppNameAndVersion.Format(hutaoOptions.Version);
        window.AppWindow.SetIcon(Path.Combine(hutaoOptions.InstalledLocation, "Assets/Logo.ico"));
        ExtendsContentIntoTitleBar();

        RecoverOrInitWindowSize();
        UpdateImmersiveDarkMode(options.TitleBar, default!);

        // appWindow.Show(true);
        // appWindow.Show can't bring window to top.
        window.Activate();
        options.BringToForeground();

        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();
        UpdateSystemBackdrop(appOptions.BackdropType);
        appOptions.PropertyChanged += OnOptionsPropertyChanged;

        subclass.Initialize();

        window.Closed += OnWindowClosed;
        options.TitleBar.ActualThemeChanged += UpdateImmersiveDarkMode;
    }

    private void RecoverOrInitWindowSize()
    {
        // Set first launch size
        double scale = options.GetWindowScale();
        SizeInt32 scaledSize = options.InitSize.Scale(scale);
        RectInt32 rect = StructMarshal.RectInt32(scaledSize);

        if (options.PersistSize)
        {
            RectInt32 persistedRect = (CompactRect)LocalSetting.Get(SettingKeys.WindowRect, (CompactRect)rect);
            if (persistedRect.Size() >= options.InitSize.Size())
            {
                rect = persistedRect.Scale(scale);
            }
        }

        TransformToCenterScreen(ref rect);
        window.AppWindow.MoveAndResize(rect);
    }

    private void SaveOrSkipWindowSize()
    {
        if (!options.PersistSize)
        {
            return;
        }

        WINDOWPLACEMENT windowPlacement = Win32.StructMarshal.WINDOWPLACEMENT();
        GetWindowPlacement(options.Hwnd, ref windowPlacement);

        // prevent save value when we are maximized.
        if (!windowPlacement.showCmd.HasFlag(SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED))
        {
            double scale = 1 / options.GetWindowScale();
            LocalSetting.Set(SettingKeys.WindowRect, (CompactRect)window.AppWindow.GetRect().Scale(scale));
        }
    }

    private void OnOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppOptions.BackdropType))
        {
            if (sender is AppOptions options)
            {
                UpdateSystemBackdrop(options.BackdropType);
            }
        }
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        SaveOrSkipWindowSize();
        subclass?.Dispose();
    }

    private void ExtendsContentIntoTitleBar()
    {
        if (options.UseLegacyDragBarImplementation)
        {
            // use normal Window method to extend.
            window.ExtendsContentIntoTitleBar = true;
            window.SetTitleBar(options.TitleBar);
        }
        else
        {
            AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;
            appTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            appTitleBar.ExtendsContentIntoTitleBar = true;

            UpdateTitleButtonColor();
            UpdateDragRectangles();
            options.TitleBar.ActualThemeChanged += (_, _) => UpdateTitleButtonColor();
            options.TitleBar.SizeChanged += (_, _) => UpdateDragRectangles();
        }
    }

    private void UpdateSystemBackdrop(BackdropType backdropType)
    {
        window.SystemBackdrop = backdropType switch
        {
            BackdropType.MicaAlt => new MicaBackdrop() { Kind = MicaKind.BaseAlt },
            BackdropType.Mica => new MicaBackdrop() { Kind = MicaKind.Base },
            BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
            _ => null,
        };
    }

    private void UpdateTitleButtonColor()
    {
        AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;

        appTitleBar.ButtonBackgroundColor = Colors.Transparent;
        appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        IAppResourceProvider resourceProvider = serviceProvider.GetRequiredService<IAppResourceProvider>();

        Color systemBaseLowColor = resourceProvider.GetResource<Color>("SystemBaseLowColor");
        appTitleBar.ButtonHoverBackgroundColor = systemBaseLowColor;

        Color systemBaseMediumLowColor = resourceProvider.GetResource<Color>("SystemBaseMediumLowColor");
        appTitleBar.ButtonPressedBackgroundColor = systemBaseMediumLowColor;

        // The Foreground doesn't accept Alpha channel. So we translate it to gray.
        byte light = (byte)((systemBaseMediumLowColor.R + systemBaseMediumLowColor.G + systemBaseMediumLowColor.B) / 3);
        byte result = (byte)((systemBaseMediumLowColor.A / 255.0) * light);
        appTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xFF, result, result, result);

        Color systemBaseHighColor = resourceProvider.GetResource<Color>("SystemBaseHighColor");
        appTitleBar.ButtonForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonHoverForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonPressedForegroundColor = systemBaseHighColor;
    }

    private unsafe void UpdateImmersiveDarkMode(FrameworkElement titleBar, object discard)
    {
        BOOL isDarkMode = Control.Theme.ThemeHelper.IsDarkMode(titleBar.ActualTheme);
        DwmSetWindowAttribute(options.Hwnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &isDarkMode, unchecked((uint)sizeof(BOOL)));
    }

    private void UpdateDragRectangles()
    {
        AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;

        double scale = options.GetWindowScale();

        // 48 is the navigation button leftInset
        RectInt32 dragRect = StructMarshal.RectInt32(48, 0, options.TitleBar.ActualSize).Scale(scale);
        appTitleBar.SetDragRectangles(dragRect.ToArray());
    }
}