// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao;

/// <summary>
/// 指引窗口
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed partial class GuideWindow : Window, IXamlWindowOptionsSource, IMinMaxInfoHandler
{
    private const int MinWidth = 1000;
    private const int MinHeight = 650;

    private const int MaxWidth = 1200;
    private const int MaxHeight = 800;

    private readonly XamlWindowOptions windowOptions;

    public GuideWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        windowOptions = new(this, DragableGrid, new(MinWidth, MinHeight), SettingKeys.GuideWindowRect);
        this.InitializeController(serviceProvider);
    }

    XamlWindowOptions IXamlWindowOptionsSource.WindowOptions { get => windowOptions; }

    public unsafe void HandleMinMaxInfo(ref MINMAXINFO info, double scalingFactor)
    {
        info.ptMinTrackSize.x = (int)Math.Max(MinWidth * scalingFactor, info.ptMinTrackSize.x);
        info.ptMinTrackSize.y = (int)Math.Max(MinHeight * scalingFactor, info.ptMinTrackSize.y);
        info.ptMaxTrackSize.x = (int)Math.Min(MaxWidth * scalingFactor, info.ptMaxTrackSize.x);
        info.ptMaxTrackSize.y = (int)Math.Min(MaxHeight * scalingFactor, info.ptMaxTrackSize.y);
    }
}