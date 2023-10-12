// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;
using Windows.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton)]
[SuppressMessage("", "CA1001")]
internal sealed partial class MainWindow : Window, IWindowOptionsSource, IMinMaxInfoHandler
{
    private const int MinWidth = 848;
    private const int MinHeight = 524;

    private readonly WindowOptions windowOptions;
    private readonly ILogger<MainWindow> logger;
    private readonly TypedEventHandler<object, WindowEventArgs> closedEventHander;
    private readonly TypedEventHandler<object, WindowSizeChangedEventArgs> sizeChangedEventHandler;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        windowOptions = new(this, TitleBarView.DragArea, new(1200, 741), true);
        this.InitializeController(serviceProvider);
        logger = serviceProvider.GetRequiredService<ILogger<MainWindow>>();

        closedEventHander = OnClosed;
        sizeChangedEventHandler = OnSizeChanged;

        Closed += closedEventHander;
        SizeChanged += sizeChangedEventHandler;
    }

    /// <inheritdoc/>
    public WindowOptions WindowOptions { get => windowOptions; }

    /// <inheritdoc/>
    public unsafe void HandleMinMaxInfo(ref MINMAXINFO pInfo, double scalingFactor)
    {
        pInfo.ptMinTrackSize.X = (int)Math.Max(MinWidth * scalingFactor, pInfo.ptMinTrackSize.X);
        pInfo.ptMinTrackSize.Y = (int)Math.Max(MinHeight * scalingFactor, pInfo.ptMinTrackSize.Y);
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        logger.LogInformation("MainWindow Closed");
    }

    private void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
    }
}