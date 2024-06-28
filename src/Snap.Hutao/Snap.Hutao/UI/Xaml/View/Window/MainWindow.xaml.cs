// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

/// <summary>
/// 主窗体
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient)]
internal sealed partial class MainWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowRectPersisted,
    IXamlWindowSubclassMinMaxInfoHandler
{
    private const int MinWidth = 1000;
    private const int MinHeight = 600;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarAccess { get => TitleBarView.DragArea; }

    public string PersistRectKey { get => SettingKeys.WindowRect; }

    public SizeInt32 InitSize { get; } = new(1200, 741);

    public SizeInt32 MinSize { get; } = new(MinWidth, MinHeight);

    /// <inheritdoc/>
    public unsafe void HandleMinMaxInfo(ref MINMAXINFO pInfo, double scalingFactor)
    {
        pInfo.ptMinTrackSize.x = (int)Math.Max(MinWidth * scalingFactor, pInfo.ptMinTrackSize.x);
        pInfo.ptMinTrackSize.y = (int)Math.Max(MinHeight * scalingFactor, pInfo.ptMinTrackSize.y);
    }
}