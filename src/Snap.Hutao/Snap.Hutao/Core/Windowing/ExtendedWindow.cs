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

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 扩展窗口
/// </summary>
/// <typeparam name="TWindow">窗体类型</typeparam>
[SuppressMessage("", "CA1001")]
internal sealed class ExtendedWindow<TWindow> : IRecipient<BackdropTypeChangedMessage>, IRecipient<FlyoutOpenCloseMessage>
    where TWindow : Window, IExtendedWindowSource
{
    private readonly WindowOptions<TWindow> options;

    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ExtendedWindow<TWindow>> logger;
    private readonly WindowSubclass<TWindow> subclass;

    private SystemBackdrop? systemBackdrop;

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
        UpdateDragRectangles(options.AppWindow.TitleBar, message.IsOpen);
    }

    private void InitializeWindow()
    {
        options.AppWindow.Title = string.Format(SH.AppNameAndVersion, CoreEnvironment.Version);
        options.AppWindow.SetIcon(Path.Combine(CoreEnvironment.InstalledLocation, "Assets/Logo.ico"));
        ExtendsContentIntoTitleBar();

        Persistence.RecoverOrInit(options);

        // appWindow.Show(true);
        // appWindow.Show can't bring window to top.
        options.Window.Activate();

        systemBackdrop = new(options.Window);
        bool micaApplied = systemBackdrop.TryApply();
        logger.LogInformation("Apply {name} : {result}", nameof(SystemBackdrop), micaApplied ? "succeed" : "failed");

        bool subClassApplied = subclass.Initialize();
        logger.LogInformation("Apply {name} : {result}", nameof(WindowSubclass<TWindow>), subClassApplied ? "succeed" : "failed");

        IMessenger messenger = serviceProvider.GetRequiredService<IMessenger>();
        messenger.Register<BackdropTypeChangedMessage>(this);
        messenger.Register<FlyoutOpenCloseMessage>(this);

        options.Window.Closed += OnWindowClosed;
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

            UpdateTitleButtonColor(appTitleBar);
            UpdateDragRectangles(appTitleBar);
            options.TitleBar.ActualThemeChanged += (s, e) => UpdateTitleButtonColor(appTitleBar);
            options.TitleBar.SizeChanged += (s, e) => UpdateDragRectangles(appTitleBar);
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