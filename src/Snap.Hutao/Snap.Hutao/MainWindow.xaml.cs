// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;
using Windows.Graphics;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[Injection(InjectAs.Singleton)]
[SuppressMessage("", "CA1001")]
public sealed partial class MainWindow : Window, IExtendedWindowSource
{
    private const int MinWidth = 848;
    private const int MinHeight = 524;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        ExtendedWindow<MainWindow>.Initialize(this);
        IsPresent = true;
        Closed += (s, e) => IsPresent = false;
    }

    /// <summary>
    /// 是否打开
    /// </summary>
    public static bool IsPresent { get; private set; }

    /// <inheritdoc/>
    public FrameworkElement TitleBar { get => TitleBarView.DragArea; }

    /// <inheritdoc/>
    public bool PersistSize { get => true; }

    /// <inheritdoc/>
    public SizeInt32 InitSize { get => new(1200, 741); }

    /// <inheritdoc/>
    public unsafe void ProcessMinMaxInfo(MINMAXINFO* pInfo, double scalingFactor)
    {
        pInfo->ptMinTrackSize.X = (int)Math.Max(MinWidth * scalingFactor, pInfo->ptMinTrackSize.X);
        pInfo->ptMinTrackSize.Y = (int)Math.Max(MinHeight * scalingFactor, pInfo->ptMinTrackSize.Y);
    }
}