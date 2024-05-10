// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton)]
[SuppressMessage("", "CA1001")]
internal sealed partial class MainWindow : Window, IWindowOptionsSource, IMinMaxInfoHandler
{
    private const int MinWidth = 1000;
    private const int MinHeight = 600;

    private readonly WindowOptions windowOptions;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        windowOptions = new(this, TitleBarView.DragArea, new(1200, 741), SettingKeys.WindowRect);
        this.InitializeController(serviceProvider);
    }

    /// <inheritdoc/>
    public WindowOptions WindowOptions { get => windowOptions; }

    /// <inheritdoc/>
    public unsafe void HandleMinMaxInfo(ref MINMAXINFO pInfo, double scalingFactor)
    {
        pInfo.ptMinTrackSize.x = (int)Math.Max(MinWidth * scalingFactor, pInfo.ptMinTrackSize.x);
        pInfo.ptMinTrackSize.y = (int)Math.Max(MinHeight * scalingFactor, pInfo.ptMinTrackSize.y);
    }
}