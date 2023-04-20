// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Message;
using Snap.Hutao.Service;
using Snap.Hutao.Win32;
using System.IO;
using Windows.Win32.Foundation;
using Windows.Graphics;
using Windows.UI;
using Windows.Win32.Graphics.Dwm;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 扩展窗口
/// </summary>
/// <typeparam name="TWindow">窗体类型</typeparam>
[SuppressMessage("", "CA1001")]
internal sealed class ExtendedWindow<TWindow> : IRecipient<FlyoutOpenCloseMessage>
    where TWindow : Window, IExtendedWindowSource
{
    private readonly WindowOptions<TWindow> options;

    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ExtendedWindow<TWindow>> logger;
    private readonly WindowSubclass<TWindow> subclass;

    private ExtendedWindow(TWindow window, FrameworkElement titleBar, IServiceProvider serviceProvider)
    {
        options = new(window, titleBar);
        subclass = new(options);

        logger = serviceProvider.GetRequiredService<ILogger<ExtendedWindow<TWindow>>>();
        this.serviceProvider = serviceProvider;

        InitializeWindow();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="serviceProvider">服务提供器</param>
    /// <returns>实例</returns>
    public static ExtendedWindow<TWindow> Initialize(TWindow window, IServiceProvider serviceProvider)
    {
        return new(window, window.TitleBar, serviceProvider);
    }

    /// <inheritdoc/>
    public void Receive(FlyoutOpenCloseMessage message)
    {
        UpdateDragRectangles(message.IsOpen);
    }

    private void InitializeWindow()
    {
        options.AppWindow.Title = string.Format(SH.AppNameAndVersion, CoreEnvironment.Version);
        options.AppWindow.SetIcon(Path.Combine(CoreEnvironment.InstalledLocation, "Assets/Logo.ico"));
        ExtendsContentIntoTitleBar();

        Persistence.RecoverOrInit(options);
        UpdateImmersiveDarkMode(options.TitleBar, default!);

        // appWindow.Show(true);
        // appWindow.Show can't bring window to top.
        // options.Window.Activate();
        Persistence.BringToForeground(options.Hwnd);

        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();
        UpdateSystemBackdrop(appOptions.BackdropType);
        appOptions.PropertyChanged += OnOptionsPropertyChanged;

        bool subClassApplied = subclass.Initialize();

        serviceProvider.GetRequiredService<IMessenger>().Register(this);

        options.Window.Closed += OnWindowClosed;
        options.TitleBar.ActualThemeChanged += UpdateImmersiveDarkMode;
    }

    private void OnOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppOptions.BackdropType))
        {
            UpdateSystemBackdrop(((AppOptions)sender!).BackdropType);
        }
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        if (options.Window.PersistSize)
        {
            Persistence.Save(options);
        }

        subclass?.Dispose();
    }

    private void ExtendsContentIntoTitleBar()
    {
        if (options.UseLegacyDragBarImplementation)
        {
            // use normal Window method to extend.
            options.Window.ExtendsContentIntoTitleBar = true;
            options.Window.SetTitleBar(options.TitleBar);
        }
        else
        {
            AppWindowTitleBar appTitleBar = options.AppWindow.TitleBar;
            appTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            appTitleBar.ExtendsContentIntoTitleBar = true;

            UpdateTitleButtonColor();
            UpdateDragRectangles();
            options.TitleBar.ActualThemeChanged += (s, e) => UpdateTitleButtonColor();
            options.TitleBar.SizeChanged += (s, e) => UpdateDragRectangles();
        }
    }

    private void UpdateSystemBackdrop(BackdropType backdropType)
    {
        options.Window.SystemBackdrop = backdropType switch
        {
            BackdropType.MicaAlt => new MicaBackdrop() { Kind = MicaKind.BaseAlt },
            BackdropType.Mica => new MicaBackdrop() { Kind = MicaKind.Base },
            BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
            _ => null,
        };
    }

    private void UpdateTitleButtonColor()
    {
        AppWindowTitleBar appTitleBar = options.AppWindow.TitleBar;

        appTitleBar.ButtonBackgroundColor = Colors.Transparent;
        appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        App app = serviceProvider.GetRequiredService<App>();

        Color systemBaseLowColor = (Color)app.Resources["SystemBaseLowColor"];
        appTitleBar.ButtonHoverBackgroundColor = systemBaseLowColor;

        Color systemBaseMediumLowColor = (Color)app.Resources["SystemBaseMediumLowColor"];
        appTitleBar.ButtonPressedBackgroundColor = systemBaseMediumLowColor;

        // The Foreground doesn't accept Alpha channel. So we translate it to gray.
        byte light = (byte)((systemBaseMediumLowColor.R + systemBaseMediumLowColor.G + systemBaseMediumLowColor.B) / 3);
        byte result = (byte)((systemBaseMediumLowColor.A / 255.0) * light);
        appTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xFF, result, result, result);

        Color systemBaseHighColor = (Color)app.Resources["SystemBaseHighColor"];
        appTitleBar.ButtonForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonHoverForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonPressedForegroundColor = systemBaseHighColor;
    }

    private unsafe void UpdateImmersiveDarkMode(FrameworkElement titleBar, object discard)
    {
        BOOL isDarkMode = Control.Theme.ThemeHelper.IsDarkMode(titleBar.ActualTheme);
        DwmSetWindowAttribute(options.Hwnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &isDarkMode, unchecked((uint)sizeof(BOOL)));
    }

    private void UpdateDragRectangles(bool isFlyoutOpened = false)
    {
        AppWindowTitleBar appTitleBar = options.AppWindow.TitleBar;

        if (isFlyoutOpened)
        {
            // set to 0
            appTitleBar.SetDragRectangles(default(RectInt32).ToArray());
        }
        else
        {
            double scale = Persistence.GetScaleForWindowHandle(options.Hwnd);

            // 48 is the navigation button leftInset
            RectInt32 dragRect = StructMarshal.RectInt32(new(48, 0), options.TitleBar.ActualSize).Scale(scale);
            appTitleBar.SetDragRectangles(dragRect.ToArray());

            // workaround for https://github.com/microsoft/WindowsAppSDK/issues/2976
            SizeInt32 size = options.AppWindow.ClientSize;
            size.Height -= (int)(31 * scale);
            options.AppWindow.ResizeClient(size);
        }
    }
}