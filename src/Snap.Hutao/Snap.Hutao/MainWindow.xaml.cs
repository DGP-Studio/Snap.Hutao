// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[Injection(InjectAs.Singleton)]
public sealed partial class MainWindow : Window, IDisposable
{
    private readonly WindowManager windowManager;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        windowManager = new WindowManager(this, TitleBarView.DragArea);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        windowManager.Dispose();
    }
}