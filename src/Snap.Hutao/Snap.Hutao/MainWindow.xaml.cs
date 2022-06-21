// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Context.Database;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[Injection(InjectAs.Singleton)]
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBarView.DragableArea);

        Closed += MainWindowClosed;
    }

    private void MainWindowClosed(object sender, WindowEventArgs args)
    {
        // save datebase
        Ioc.Default.GetRequiredService<AppDbContext>().SaveChanges();
    }
}