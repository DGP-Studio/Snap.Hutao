// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao;

/// <summary>
/// 指引窗口
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed partial class GuideWindow : Window, IWindowOptionsSource, IMinMaxInfoHandler
{
    private const int MinWidth = 1000;
    private const int MinHeight = 600;

    private const int MaxWidth = 1200;
    private const int MaxHeight = 750;

    private readonly WindowOptions windowOptions;

    public GuideWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        windowOptions = new(this, DragableGrid, new(MinWidth, MinHeight));
        this.InitializeController(serviceProvider);
    }

    WindowOptions IWindowOptionsSource.WindowOptions { get => windowOptions; }

    public unsafe void HandleMinMaxInfo(ref MINMAXINFO info, double scalingFactor)
    {
        info.ptMinTrackSize.X = (int)Math.Max(MinWidth * scalingFactor, info.ptMinTrackSize.X);
        info.ptMinTrackSize.Y = (int)Math.Max(MinHeight * scalingFactor, info.ptMinTrackSize.Y);
        info.ptMaxTrackSize.X = (int)Math.Min(MaxWidth * scalingFactor, info.ptMaxTrackSize.X);
        info.ptMaxTrackSize.Y = (int)Math.Min(MaxHeight * scalingFactor, info.ptMaxTrackSize.Y);
    }
}
