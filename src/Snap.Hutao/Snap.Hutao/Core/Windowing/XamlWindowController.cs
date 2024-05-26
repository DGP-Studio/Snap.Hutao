// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing.Abstraction;
using Snap.Hutao.Core.Windowing.NotifyIcon;
using Snap.Hutao.Service;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dwm;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.IO;
using Windows.Graphics;
using Windows.UI;
using static Snap.Hutao.Win32.DwmApi;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Core.Windowing;

[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SA1124")]
[SuppressMessage("", "SA1204")]
internal sealed class XamlWindowController
{
    private readonly Window window;
    private readonly IServiceProvider serviceProvider;
    private readonly XamlWindowSubclass subclass;
    private readonly XamlWindowNonRudeHWND windowNonRudeHWND;

    public XamlWindowController(Window window, IServiceProvider serviceProvider)
    {
        this.window = window;
        this.serviceProvider = serviceProvider;

        // Subclassing and NonRudeHWND are standard infrastructure.
        subclass = new(window);
        windowNonRudeHWND = new(window.GetWindowHandle());

        InitializeCore();
    }

    private void InitializeCore()
    {
        RuntimeOptions runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();

        window.AppWindow.Title = SH.FormatAppNameAndVersion(runtimeOptions.Version);
        window.AppWindow.SetIcon(Path.Combine(runtimeOptions.InstalledLocation, "Assets/Logo.ico"));

        // ExtendContentIntoTitleBar
        if (window is IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            ExtendsContentIntoTitleBar(window, xamlWindow);
        }

        // Size stuff
        if (window is IXamlWindowHasInitSize xamlWindow2)
        {
            RecoverOrInitWindowSize(xamlWindow2);
        }

        // Element Theme & Immersive Dark
        UpdateElementTheme(window, appOptions.ElementTheme);

        if (window is IXamlWindowContentAsFrameworkElement xamlWindow3)
        {
            UpdateImmersiveDarkMode(xamlWindow3.ContentAccess, default!);
            xamlWindow3.ContentAccess.ActualThemeChanged += UpdateImmersiveDarkMode;
        }

        // appWindow.Show(true);
        // appWindow.Show can't bring window to top.
        window.Activate();
        window.BringToForeground();

        // SystemBackdrop
        UpdateSystemBackdrop(appOptions.BackdropType);

        if (window.GetDesktopWindowXamlSource() is { } desktopWindowXamlSource)
        {
            DesktopChildSiteBridge desktopChildSiteBridge = desktopWindowXamlSource.SiteBridge;
            desktopChildSiteBridge.ResizePolicy = ContentSizePolicy.ResizeContentToParentWindow;
        }

        appOptions.PropertyChanged += OnOptionsPropertyChanged;

        subclass.Initialize();
        window.Closed += OnWindowClosed;
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        if (XamlWindowLifetime.ApplicationLaunchedWithNotifyIcon && !XamlWindowLifetime.ApplicationExiting)
        {
            args.Handled = true;
            window.Hide();

            if (!IsNotifyIconVisible())
            {
                new ToastContentBuilder()
                    .AddText(SH.CoreWindowingNotifyIconPromotedHint)
                    .Show();
            }

            ICurrentXamlWindowReference currentXamlWindowReference = serviceProvider.GetRequiredService<ICurrentXamlWindowReference>();
            if (currentXamlWindowReference.Window == window)
            {
                currentXamlWindowReference.Window = default!;
            }

            GC.Collect(GC.MaxGeneration);
        }
        else
        {
            if (window is IXamlWindowRectPersisted rectPersisted)
            {
                SaveOrSkipWindowSize(rectPersisted);
            }

            subclass?.Dispose();
            windowNonRudeHWND?.Dispose();
        }
    }

    private unsafe bool IsNotifyIconVisible()
    {
        RECT iconRect = serviceProvider.GetRequiredService<NotifyIconController>().GetRect();

        if (UniversalApiContract.IsPresent(WindowsVersion.Windows11))
        {
            RECT primaryRect = StructMarshal.RECT(DisplayArea.Primary.OuterBounds);
            return IntersectRect(out _, in primaryRect, in iconRect);
        }
        else
        {
            HWND shellTrayWnd = FindWindowExW(default, default, "Shell_TrayWnd", default);
            HWND trayNotifyWnd = FindWindowExW(shellTrayWnd, default, "TrayNotifyWnd", default);
            HWND button = FindWindowExW(trayNotifyWnd, default, "Button", default);

            if (GetWindowRect(button, out RECT buttonRect))
            {
                return !EqualRect(in buttonRect, in iconRect);
            }

            return false;
        }
    }

    #region SystemBackdrop & ElementTheme

    private void OnOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not AppOptions options)
        {
            return;
        }

        _ = e.PropertyName switch
        {
            nameof(AppOptions.BackdropType) => UpdateSystemBackdrop(options.BackdropType),
            nameof(AppOptions.ElementTheme) => UpdateElementTheme(window, options.ElementTheme),
            _ => false,
        };
    }

    private bool UpdateSystemBackdrop(BackdropType backdropType)
    {
        SystemBackdrop? actualBackdop = backdropType switch
        {
            BackdropType.Transparent => new Backdrop.TransparentBackdrop(),
            BackdropType.MicaAlt => new MicaBackdrop() { Kind = MicaKind.BaseAlt },
            BackdropType.Mica => new MicaBackdrop() { Kind = MicaKind.Base },
            BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
            _ => null,
        };

        window.SystemBackdrop = new Backdrop.SystemBackdropDesktopWindowXamlSourceAccess(actualBackdop);

        return true;
    }

    private static bool UpdateElementTheme(Window window, ElementTheme theme)
    {
        if (window is IXamlWindowContentAsFrameworkElement xamlWindow)
        {
            xamlWindow.ContentAccess.RequestedTheme = theme;
            return true;
        }

        if (window.Content is FrameworkElement frameworkElement)
        {
            frameworkElement.RequestedTheme = theme;
            return true;
        }

        return false;
    }
    #endregion

    #region IXamlWindowContentAsFrameworkElement

    private unsafe void UpdateImmersiveDarkMode(FrameworkElement titleBar, object discard)
    {
        BOOL isDarkMode = Control.Theme.ThemeHelper.IsDarkMode(titleBar.ActualTheme);
        DwmSetWindowAttribute(window.GetWindowHandle(), DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref isDarkMode);
    }
    #endregion

    #region IXamlWindowHasInitSize & IXamlWindowRectPersisted

    private static void TransformToCenterScreen(ref RectInt32 rect)
    {
        DisplayArea displayArea = DisplayArea.GetFromRect(rect, DisplayAreaFallback.Nearest);
        RectInt32 workAreaRect = displayArea.WorkArea;

        rect.Width = Math.Min(workAreaRect.Width, rect.Width);
        rect.Height = Math.Min(workAreaRect.Height, rect.Height);

        rect.X = workAreaRect.X + ((workAreaRect.Width - rect.Width) / 2);
        rect.Y = workAreaRect.Y + ((workAreaRect.Height - rect.Height) / 2);
    }

    private void RecoverOrInitWindowSize(IXamlWindowHasInitSize xamlWindow)
    {
        double scale = window.GetRasterizationScale();
        SizeInt32 scaledSize = xamlWindow.InitSize.Scale(scale);
        RectInt32 rect = StructMarshal.RectInt32(scaledSize);

        if (window is IXamlWindowRectPersisted rectPersisted)
        {
            RectInt32 persistedRect = (CompactRect)LocalSetting.Get(rectPersisted.PersistRectKey, (CompactRect)rect);
            if (persistedRect.Size() >= xamlWindow.InitSize.Size())
            {
                rect = persistedRect.Scale(scale);
            }
        }

        TransformToCenterScreen(ref rect);
        window.AppWindow.MoveAndResize(rect);
    }

    private void SaveOrSkipWindowSize(IXamlWindowRectPersisted rectPersisted)
    {
        WINDOWPLACEMENT windowPlacement = WINDOWPLACEMENT.Create();
        GetWindowPlacement(window.GetWindowHandle(), ref windowPlacement);

        // prevent save value when we are maximized.
        if (!windowPlacement.ShowCmd.HasFlag(SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED))
        {
            double scale = 1.0 / window.GetRasterizationScale();
            LocalSetting.Set(rectPersisted.PersistRectKey, (CompactRect)window.AppWindow.GetRect().Scale(scale));
        }
    }
    #endregion

    #region IXamlWindowExtendContentIntoTitleBar

    private void ExtendsContentIntoTitleBar(Window window, IXamlWindowExtendContentIntoTitleBar xamlWindow)
    {
        AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;
        appTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        appTitleBar.ExtendsContentIntoTitleBar = true;

        UpdateTitleButtonColor();
        xamlWindow.TitleBarAccess.ActualThemeChanged += (_, _) => UpdateTitleButtonColor();

        UpdateDragRectangles();
        xamlWindow.TitleBarAccess.SizeChanged += (_, _) => UpdateDragRectangles();
    }

    private void UpdateTitleButtonColor()
    {
        if (window is not IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            return;
        }

        AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;

        appTitleBar.ButtonBackgroundColor = Colors.Transparent;
        appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        bool isDarkMode = Control.Theme.ThemeHelper.IsDarkMode(xamlWindow.TitleBarAccess.ActualTheme);

        Color systemBaseLowColor = Control.Theme.SystemColors.BaseLowColor(isDarkMode);
        appTitleBar.ButtonHoverBackgroundColor = systemBaseLowColor;

        Color systemBaseMediumLowColor = Control.Theme.SystemColors.BaseMediumLowColor(isDarkMode);
        appTitleBar.ButtonPressedBackgroundColor = systemBaseMediumLowColor;

        // The Foreground doesn't accept Alpha channel. So we translate it to gray.
        byte light = (byte)((systemBaseMediumLowColor.R + systemBaseMediumLowColor.G + systemBaseMediumLowColor.B) / 3);
        byte result = (byte)((systemBaseMediumLowColor.A / 255.0) * light);
        appTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xFF, result, result, result);

        Color systemBaseHighColor = Control.Theme.SystemColors.BaseHighColor(isDarkMode);
        appTitleBar.ButtonForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonHoverForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonPressedForegroundColor = systemBaseHighColor;
    }

    private void UpdateDragRectangles()
    {
        if (window is not IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            return;
        }

        // 48 is the navigation button leftInset
        RectInt32 dragRect = StructMarshal.RectInt32(48, 0, xamlWindow.TitleBarAccess.ActualSize).Scale(window.GetRasterizationScale());
        window.GetInputNonClientPointerSource().SetRegionRects(NonClientRegionKind.Caption, [dragRect]);
    }
    #endregion
}