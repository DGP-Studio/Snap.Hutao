// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Core.Windowing.Abstraction;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Graphics;

namespace Snap.Hutao;

/// <summary>
/// 启动游戏窗口
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchGameWindow : Window,
    IDisposable,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize,
    IXamlWindowSubclassMinMaxInfoHandler
{
    private const int MinWidth = 240;
    private const int MinHeight = 240;

    private const int MaxWidth = 320;
    private const int MaxHeight = 320;

    private readonly IServiceScope scope;

    /// <summary>
    /// 构造一个新的启动游戏窗口
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public LaunchGameWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        scope = serviceProvider.CreateScope();

        this.InitializeController(serviceProvider);
        RootGrid.InitializeDataContext<LaunchGameViewModel>(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarAccess { get => DragableGrid; }

    public SizeInt32 InitSize { get; } = new(MaxWidth, MaxHeight);

    /// <inheritdoc/>
    public void Dispose()
    {
        scope.Dispose();
    }

    /// <inheritdoc/>
    public unsafe void HandleMinMaxInfo(ref MINMAXINFO info, double scalingFactor)
    {
        info.ptMinTrackSize.x = (int)Math.Max(MinWidth * scalingFactor, info.ptMinTrackSize.x);
        info.ptMinTrackSize.y = (int)Math.Max(MinHeight * scalingFactor, info.ptMinTrackSize.y);
        info.ptMaxTrackSize.x = (int)Math.Min(MaxWidth * scalingFactor, info.ptMaxTrackSize.x);
        info.ptMaxTrackSize.y = (int)Math.Min(MaxHeight * scalingFactor, info.ptMaxTrackSize.y);
    }
}