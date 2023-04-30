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
using Windows.Graphics;
using Windows.UI;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 扩展窗口
/// </summary>
/// <typeparam name="TWindow">窗体类型</typeparam>
[SuppressMessage("", "CA1001")]
internal sealed class ExtendedWindow<TWindow> : IRecipient<FlyoutOpenCloseMessage>
    where TWindow : Window, IWindowOptionsSource
{
    private readonly TWindow window;
    private readonly IServiceProvider serviceProvider;
    private readonly WindowSubclass<TWindow> subclass;

    private ExtendedWindow(TWindow window, IServiceProvider serviceProvider)
    {
        this.window = window;
        this.serviceProvider = serviceProvider;

        subclass = new(window);

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
        return new(window, serviceProvider);
    }

    /// <inheritdoc/>
    public void Receive(FlyoutOpenCloseMessage message)
    {
        UpdateDragRectangles(message.IsOpen);
    }

    private void InitializeWindow()
    {
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();

        WindowOptions options = window.WindowOptions;
        window.AppWindow.Title = string.Format(SH.AppNameAndVersion, hutaoOptions.Version);
        window.AppWindow.SetIcon(Path.Combine(hutaoOptions.InstalledLocation, "Assets/Logo.ico"));
        ExtendsContentIntoTitleBar();

        Persistence.RecoverOrInit(window);
        UpdateImmersiveDarkMode(options.TitleBar, default!);

        // appWindow.Show(true);
        // appWindow.Show can't bring window to top.
        window.Activate();
        Persistence.BringToForeground(options.Hwnd);

        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();
        UpdateSystemBackdrop(appOptions.BackdropType);
        appOptions.PropertyChanged += OnOptionsPropertyChanged;

        bool subClassApplied = subclass.Initialize();

        serviceProvider.GetRequiredService<IMessenger>().Register(this);

        window.Closed += OnWindowClosed;
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
        if (window.WindowOptions.PersistSize)
        {
            Persistence.Save(window);
        }

        subclass?.Dispose();
    }

    private void ExtendsContentIntoTitleBar()
    {
        WindowOptions options = window.WindowOptions;
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
            options.TitleBar.ActualThemeChanged += (s, e) => UpdateTitleButtonColor();
            options.TitleBar.SizeChanged += (s, e) => UpdateDragRectangles();
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
        DwmSetWindowAttribute(window.WindowOptions.Hwnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &isDarkMode, unchecked((uint)sizeof(BOOL)));
    }

    private void UpdateDragRectangles(bool isFlyoutOpened = false)
    {
        AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;

        if (isFlyoutOpened)
        {
            // set to 0
            appTitleBar.SetDragRectangles(default(RectInt32).ToArray());
        }
        else
        {
            WindowOptions options = window.WindowOptions;
            double scale = Persistence.GetScaleForWindowHandle(options.Hwnd);

            // 48 is the navigation button leftInset
            RectInt32 dragRect = StructMarshal.RectInt32(new(48, 0), options.TitleBar.ActualSize).Scale(scale);
            appTitleBar.SetDragRectangles(dragRect.ToArray());

            // workaround for https://github.com/microsoft/WindowsAppSDK/issues/2976
            SizeInt32 size = window.AppWindow.ClientSize;
            size.Height -= (int)(31 * scale);
            window.AppWindow.ResizeClient(size);
        }
    }
}