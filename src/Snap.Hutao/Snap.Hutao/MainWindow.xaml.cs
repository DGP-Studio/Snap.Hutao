// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[Injection(InjectAs.Singleton)]
public sealed partial class MainWindow : Window
{
    private readonly AppDbContext appDbContext;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="logger">日志器</param>
    public MainWindow(AppDbContext appDbContext, ILogger<MainWindow> logger)
    {
        this.appDbContext = appDbContext;
        InitializeComponent();
        _ = new WindowManager(this, TitleBarView.DragableArea);
    }

    private void MainWindowClosed(object sender, WindowEventArgs args)
    {
        // save userdata datebase
        int changes = appDbContext.SaveChanges();
        Verify.Operation(changes == 0, "存在未经处理的数据库记录更改");
    }
}