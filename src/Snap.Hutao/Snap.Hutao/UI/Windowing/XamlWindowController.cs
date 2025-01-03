// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppNotifications.Builder;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Shell;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dwm;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Foundation;
using Windows.Graphics;
using Windows.UI;
using static Snap.Hutao.Win32.DwmApi;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Windowing;

[SuppressMessage("", "CA1001")]
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

    public void UpdateDragRectangles()
    {
        PrivateUpdateDragRectangles();
    }

    private void InitializeCore()
    {
        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();

        window.AppWindow.Title = SH.FormatAppNameAndVersion(HutaoRuntime.Version);
        window.AppWindow.SetIcon(InstalledLocation.GetAbsolutePath("Assets/Logo.ico"));

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

        // Immersive Dark
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
        if (args.Handled)
        {
            return;
        }

        serviceProvider.GetRequiredService<AppOptions>().PropertyChanged -= OnOptionsPropertyChanged;

        if (XamlApplicationLifetime.LaunchedWithNotifyIcon && !XamlApplicationLifetime.Exiting)
        {
            ICurrentXamlWindowReference currentXamlWindowReference = serviceProvider.GetRequiredService<ICurrentXamlWindowReference>();
            if (currentXamlWindowReference.Window == window)
            {
                currentXamlWindowReference.Window = default!;

                if (!IsNotifyIconVisible())
                {
                    try
                    {
                        new AppNotificationBuilder()
                            .AddText(SH.CoreWindowingNotifyIconPromotedHint)
                            .Show();
                    }
                    catch
                    {
                    }
                }
            }

            GC.Collect(GC.MaxGeneration);
        }

        if (window is IXamlWindowRectPersisted rectPersisted)
        {
            SaveOrSkipWindowSize(rectPersisted);
        }

        subclass?.Dispose();
        windowNonRudeHWND?.Dispose();

        if (window is IXamlWindowClosedHandler xamlWindowClosed)
        {
            xamlWindowClosed.OnWindowClosed();
        }

        window.UninitializeController();
    }

    private bool IsNotifyIconVisible()
    {
        try
        {
            NotifyIconController notifyIconController = serviceProvider.LockAndGetRequiredService<NotifyIconController>(NotifyIconController.InitializationSyncRoot);

            // Actual version should be above 24H2 (26100), which is 26120 without UniversalApiContract.
            if (Core.UniversalApiContract.IsPresent(WindowsVersion.Windows11Version24H2))
            {
                return notifyIconController.GetIsPromoted();
            }

            // Shell_NotifyIconGetRect can return E_FAIL in multiple cases, so we use the fallback method.
            RECT iconRect = notifyIconController.GetRect();
            if (Core.UniversalApiContract.IsPresent(WindowsVersion.Windows11))
            {
                RECT primaryRect = DisplayArea.Primary.OuterBounds.ToRECT();
                return IntersectRect(out _, in primaryRect, in iconRect);
            }

            HWND shellTrayWnd = FindWindowExW(default, default, "Shell_TrayWnd", default);
            HWND trayNotifyWnd = FindWindowExW(shellTrayWnd, default, "TrayNotifyWnd", default);
            HWND button = FindWindowExW(trayNotifyWnd, default, "Button", default);

            if (GetWindowRect(button, out RECT buttonRect))
            {
                return !EqualRect(in buttonRect, in iconRect);
            }

            return false;
        }
        catch
        {
#if DEBUG
            throw;
#else
            return false;
#endif
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
            _ => false,
        };
    }

    private bool UpdateSystemBackdrop(BackdropType backdropType)
    {
        SystemBackdrop? actualBackdop = backdropType switch
        {
            BackdropType.Transparent => new TransparentBackdrop(),
            BackdropType.MicaAlt => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            BackdropType.Mica => new MicaBackdrop { Kind = MicaKind.Base },
            BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
            _ => null,
        };

        window.SystemBackdrop = new SystemBackdropDesktopWindowXamlSourceAccess(actualBackdop);

        return true;
    }
    #endregion

    #region IXamlWindowContentAsFrameworkElement

    private void UpdateImmersiveDarkMode(FrameworkElement titleBar, object discard)
    {
        BOOL isDarkMode = ThemeHelper.IsDarkMode(titleBar.ActualTheme);
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
        RectInt32 rect = xamlWindow.InitSize.ToRectInt32();

        if (window is IXamlWindowRectPersisted rectPersisted)
        {
            double scale = LocalSetting.Get(rectPersisted.PersistScaleKey, 0.0);

            // Never persisted before
            if (scale == 0.0)
            {
                // Move to the primary screen and get the scale
                window.AppWindow.Move(DisplayArea.Primary.WorkArea.GetPointInt32(PointInt32Kind.TopLeft));
                scale = window.GetRasterizationScale();
            }

            // DO NOT INLINE, implicit conversion requires a local variable.
            RectInt32 nonDpiPersistedRect = (RectInt16)LocalSetting.Get(rectPersisted.PersistRectKey, 0UL);
            RectInt32 persistedRect = nonDpiPersistedRect.Scale(scale);

            // If the persisted size is less than min size, we want to reset to the init size.
            SizeInt32 scaledMinSize = xamlWindow.MinSize.Scale(scale);
            if (persistedRect.Width < scaledMinSize.Width || persistedRect.Height < scaledMinSize.Height)
            {
                rect = scaledMinSize.ToRectInt32();
            }
            else
            {
                rect = persistedRect;
            }
        }

        TransformToCenterScreen(ref rect);
        window.AppWindow.MoveThenResize(rect);
    }

    private void SaveOrSkipWindowSize(IXamlWindowRectPersisted rectPersisted)
    {
        WINDOWPLACEMENT windowPlacement = WINDOWPLACEMENT.Create();
        GetWindowPlacement(window.GetWindowHandle(), ref windowPlacement);

        // prevent save value when we are maximized.
        if (windowPlacement.ShowCmd.HasFlag(SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED))
        {
            return;
        }

        // We save the non-dpi rect here
        double scale = window.GetRasterizationScale();
        LocalSetting.Set(rectPersisted.PersistScaleKey, scale);

        // DO NOT INLINE, implicit conversion requires a local variable.
        RectInt16 rect = (RectInt16)window.AppWindow.GetRect().Scale(1.0 / scale);
        if (rect.Width < 0 || rect.Height < 0)
        {
            return;
        }

        LocalSetting.Set(rectPersisted.PersistRectKey, rect);
    }
    #endregion

    #region IXamlWindowExtendContentIntoTitleBar

    private void ExtendsContentIntoTitleBar(Window window, IXamlWindowExtendContentIntoTitleBar xamlWindow)
    {
        AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;
        appTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        appTitleBar.ExtendsContentIntoTitleBar = true;

        UpdateTitleButtonColor();
        xamlWindow.TitleBarCaptionAccess.ActualThemeChanged += (_, _) => UpdateTitleButtonColor();

        PrivateUpdateDragRectangles();
        xamlWindow.TitleBarCaptionAccess.SizeChanged += (_, _) => PrivateUpdateDragRectangles();
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

        bool isDarkMode = ThemeHelper.IsDarkMode(xamlWindow.TitleBarCaptionAccess.ActualTheme);

        Color systemBaseLowColor = SystemColors.BaseLowColor(isDarkMode);
        appTitleBar.ButtonHoverBackgroundColor = systemBaseLowColor;

        Color systemBaseMediumLowColor = SystemColors.BaseMediumLowColor(isDarkMode);
        appTitleBar.ButtonPressedBackgroundColor = systemBaseMediumLowColor;

        // The Foreground doesn't accept Alpha channel. So we translate it to gray.
        byte light = (byte)((systemBaseMediumLowColor.R + systemBaseMediumLowColor.G + systemBaseMediumLowColor.B) / 3);
        byte result = (byte)(systemBaseMediumLowColor.A / 255.0 * light);
        appTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xFF, result, result, result);

        Color systemBaseHighColor = SystemColors.BaseHighColor(isDarkMode);
        appTitleBar.ButtonForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonHoverForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonPressedForegroundColor = systemBaseHighColor;
    }

    private void PrivateUpdateDragRectangles()
    {
        if (window is not IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            return;
        }

        // E_UNEXPECTED will be thrown if the Content is not loaded.
        if (xamlWindow.TitleBarCaptionAccess.IsLoaded)
        {
            InputNonClientPointerSource inputNonClientPointerSource = window.GetInputNonClientPointerSource();
            {
                FrameworkElement element = xamlWindow.TitleBarCaptionAccess;
                Point position = element.TransformToVisual(window.Content).TransformPoint(default);
                RectInt32 rect = RectInt32Convert.RectInt32(position, element.ActualSize).Scale(window.GetRasterizationScale());
                inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Caption, [rect]);
            }

            List<RectInt32> passthrough = [];
            foreach (FrameworkElement element in xamlWindow.TitleBarPassthrough)
            {
                if (element.Visibility is not Visibility.Visible)
                {
                    continue;
                }

                Point position = element.TransformToVisual(window.Content).TransformPoint(default);
                RectInt32 rect = RectInt32Convert.RectInt32(position, element.ActualSize).Scale(window.GetRasterizationScale());

                if (rect.Size() > 0)
                {
                    passthrough.Add(rect);
                }
            }

            if (passthrough.Count > 0)
            {
                inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Passthrough, [.. passthrough]);
            }
        }
    }
    #endregion
}