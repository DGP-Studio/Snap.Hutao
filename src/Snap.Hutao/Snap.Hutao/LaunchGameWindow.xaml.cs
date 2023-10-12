// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.ViewModel.Game;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao;

/// <summary>
/// 启动游戏窗口
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchGameWindow : Window, IDisposable, IWindowOptionsSource, IMinMaxInfoHandler
{
    private const int MinWidth = 240;
    private const int MinHeight = 240;

    private const int MaxWidth = 320;
    private const int MaxHeight = 320;

    private readonly WindowOptions windowOptions;
    private readonly IServiceScope scope;

    /// <summary>
    /// 构造一个新的启动游戏窗口
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public LaunchGameWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        scope = serviceProvider.CreateScope();
        windowOptions = new(this, DragableGrid, new(MaxWidth, MaxHeight));
        this.InitializeController(serviceProvider);
        RootGrid.DataContext = scope.ServiceProvider.GetRequiredService<LaunchGameViewModel>();
    }

    /// <inheritdoc/>
    public WindowOptions WindowOptions { get => windowOptions; }

    /// <inheritdoc/>
    public void Dispose()
    {
        scope.Dispose();
    }

    /// <inheritdoc/>
    public unsafe void HandleMinMaxInfo(ref MINMAXINFO info, double scalingFactor)
    {
        info.ptMinTrackSize.X = (int)Math.Max(MinWidth * scalingFactor, info.ptMinTrackSize.X);
        info.ptMinTrackSize.Y = (int)Math.Max(MinHeight * scalingFactor, info.ptMinTrackSize.Y);
        info.ptMaxTrackSize.X = (int)Math.Min(MaxWidth * scalingFactor, info.ptMaxTrackSize.X);
        info.ptMaxTrackSize.Y = (int)Math.Min(MaxHeight * scalingFactor, info.ptMaxTrackSize.Y);
    }
}