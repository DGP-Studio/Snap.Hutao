// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[Injection(InjectAs.Singleton)]
[SuppressMessage("", "CA1001")]
public sealed partial class MainWindow : Window
{
    private readonly WindowManager windowManager;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Closed += MainWindowClosed;
        windowManager = new WindowManager(this, TitleBarView.DragArea);
    }

    private void MainWindowClosed(object sender, WindowEventArgs args)
    {
        // Must dispose it before window is completely closed
        windowManager?.Dispose();
    }
}