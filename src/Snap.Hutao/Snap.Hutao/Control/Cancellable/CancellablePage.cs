// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Snap.Hutao.Control.Cancellable;

/// <summary>
/// 表示支持取消加载的异步页面
/// 在被导航到其他页面前触发取消异步通知
/// </summary>
public class CancellablePage : Page
{
    private readonly CancellationTokenSource viewLoadingConcellationTokenSource = new();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <typeparam name="TViewModel">视图模型类型</typeparam>
    public void InitializeWith<TViewModel>()
        where TViewModel : class, ISupportCancellation
    {
        ISupportCancellation viewModel = Ioc.Default.GetRequiredService<TViewModel>();
        viewModel.CancellationToken = viewLoadingConcellationTokenSource.Token;
        DataContext = viewModel;
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        viewLoadingConcellationTokenSource.Cancel();
    }
}