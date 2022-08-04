// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Windowing;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[Injection(InjectAs.Singleton)]
public sealed partial class MainWindow : Window
{
    private readonly AppDbContext appDbContext;
    private readonly WindowManager windowManager;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="logger">日志器</param>
    public MainWindow(AppDbContext appDbContext, ILogger<MainWindow> logger)
    {
        this.appDbContext = appDbContext;
        InitializeComponent();
        windowManager = new WindowManager(this, (FrameworkElement)TitleBarView.DragableArea);
    }

    private void MainWindowClosed(object sender, WindowEventArgs args)
    {
        windowManager.Dispose();

        // save userdata datebase
        int changes = appDbContext.SaveChanges();
        Verify.Operation(changes == 0, "存在未经处理的数据库记录更改");
    }
}