// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Message;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[Injection(InjectAs.Singleton)]
public sealed partial class MainWindow : Window
{
    private readonly AppDbContext appDbContext;
    private readonly WindowManager windowManager;
    private readonly IMessenger messenger;

    private readonly TaskCompletionSource initializaionCompletionSource = new();

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="messenger">消息器</param>
    public MainWindow(AppDbContext appDbContext, IMessenger messenger)
    {
        this.appDbContext = appDbContext;
        this.messenger = messenger;

        InitializeComponent();
        windowManager = new WindowManager(this, TitleBarView.DragableArea);

        initializaionCompletionSource.TrySetResult();
    }

    private void MainWindowClosed(object sender, WindowEventArgs args)
    {
        messenger.Send(new MainWindowClosedMessage());

        windowManager?.Dispose();

        // save userdata datebase
        int changes = appDbContext.SaveChanges();
        Verify.Operation(changes == 0, "存在未经处理的数据库记录更改");
    }
}