// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Message;
using Snap.Hutao.Win32;
using System.IO;
using Windows.Graphics;
using Windows.UI;
using Windows.Win32.Foundation;
using WinRT.Interop;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 扩展窗口
/// </summary>
/// <typeparam name="TWindow">窗体类型</typeparam>
[SuppressMessage("", "CA1001")]
internal sealed class ExtendedWindow<TWindow> : IRecipient<BackdropTypeChangedMessage>, IRecipient<FlyoutOpenCloseMessage>
    where TWindow : Window, IExtendedWindowSource
{
    private readonly HWND hwnd;
    private readonly AppWindow appWindow;

    private readonly TWindow window;
    private readonly FrameworkElement titleBar;

    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ExtendedWindow<TWindow>> logger;
    private readonly WindowSubclassManager<TWindow> subclassManager;

    private readonly bool useLegacyDragBar;

    private SystemBackdrop? systemBackdrop;

    private ExtendedWindow(TWindow window, FrameworkElement titleBar, IServiceProvider serviceProvider)
    {
        this.window = window;
        this.titleBar = titleBar;
        logger = serviceProvider.GetRequiredService<ILogger<ExtendedWindow<TWindow>>>();
        this.serviceProvider = serviceProvider;

        hwnd = (HWND)WindowNative.GetWindowHandle(window);
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        appWindow = AppWindow.GetFromWindowId(windowId);

        useLegacyDragBar = !AppWindowTitleBar.IsCustomizationSupported();
        subclassManager = new(window, hwnd, useLegacyDragBar);

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
    public void Receive(BackdropTypeChangedMessage message)
    {
        if (systemBackdrop != null)
        {
            systemBackdrop.BackdropType = message.BackdropType;
            bool micaApplied = systemBackdrop.TryApply();
            logger.LogInformation("Apply {name} : {result}", nameof(SystemBackdrop), micaApplied ? "succeed" : "failed");
        }
    }

    /// <inheritdoc/>
    public void Receive(FlyoutOpenCloseMessage message)
    {
        UpdateDragRectangles(appWindow.TitleBar, message.IsOpen);
    }

    private void InitializeWindow()
    {
        appWindow.Title = string.Format(SH.AppNameAndVersion, CoreEnvironment.Version);
        appWindow.SetIcon(Path.Combine(CoreEnvironment.InstalledLocation, "Assets/Logo.ico"));
        ExtendsContentIntoTitleBar();

        Persistence.RecoverOrInit(appWindow, window.PersistSize, window.InitSize);

        // appWindow.Show(true);
        // appWindow.Show can't bring window to top.
        window.Activate();

        systemBackdrop = new(window);
        bool micaApplied = systemBackdrop.TryApply();
        logger.LogInformation("Apply {name} : {result}", nameof(SystemBackdrop), micaApplied ? "succeed" : "failed");

        bool subClassApplied = subclassManager.TrySetWindowSubclass();
        logger.LogInformation("Apply {name} : {result}", nameof(WindowSubclassManager<TWindow>), subClassApplied ? "succeed" : "failed");

        IMessenger messenger = Ioc.Default.GetRequiredService<IMessenger>();
        messenger.Register<BackdropTypeChangedMessage>(this);
        messenger.Register<FlyoutOpenCloseMessage>(this);

        window.Closed += OnWindowClosed;
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        if (window.PersistSize)
        {
            Persistence.Save(appWindow);
        }

        subclassManager?.Dispose();
    }

    private void ExtendsContentIntoTitleBar()
    {
        if (useLegacyDragBar)
        {
            // use normal Window method to extend.
            window.ExtendsContentIntoTitleBar = true;
            window.SetTitleBar(titleBar);
        }
        else
        {
            AppWindowTitleBar appTitleBar = appWindow.TitleBar;
            appTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            appTitleBar.ExtendsContentIntoTitleBar = true;

            UpdateTitleButtonColor(appTitleBar);
            UpdateDragRectangles(appTitleBar);
            titleBar.ActualThemeChanged += (s, e) => UpdateTitleButtonColor(appTitleBar);
            titleBar.SizeChanged += (s, e) => UpdateDragRectangles(appTitleBar);
        }
    }

    private void UpdateTitleButtonColor(AppWindowTitleBar appTitleBar)
    {
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

    private void UpdateDragRectangles(AppWindowTitleBar appTitleBar, bool isFlyoutOpened = false)
    {
        if (isFlyoutOpened)
        {
            // set to 0
            appTitleBar.SetDragRectangles(default(RectInt32).Enumerate().ToArray());
        }
        else
        {
            double scale = Persistence.GetScaleForWindowHandle(hwnd);

            // 48 is the navigation button leftInset
            RectInt32 dragRect = StructMarshal.RectInt32(new(48, 0), titleBar.ActualSize).Scale(scale);
            appTitleBar.SetDragRectangles(dragRect.Enumerate().ToArray());

            // workaround for https://github.com/microsoft/WindowsAppSDK/issues/2976
            SizeInt32 size = appWindow.ClientSize;
            size.Height -= (int)(31 * scale);
            appWindow.ResizeClient(size);
        }
    }
}