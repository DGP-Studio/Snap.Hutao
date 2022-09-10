// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Snap.Hutao.Control;

/// <summary>
/// 表示支持取消加载的异步页面
/// 在被导航到其他页面前触发取消异步通知
/// </summary>
public class ScopedPage : Page
{
    private readonly CancellationTokenSource viewLoadingCancellationTokenSource = new();
    private readonly IServiceScope serviceScope;

    /// <summary>
    /// 构造一个新的页面
    /// </summary>
    public ScopedPage()
    {
        serviceScope = Ioc.Default.CreateScope();
    }

    /// <inheritdoc cref="IServiceScope.ServiceProvider"/>
    public IServiceProvider ServiceProvider { get => serviceScope.ServiceProvider; }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <typeparam name="TViewModel">视图模型类型</typeparam>
    public void InitializeWith<TViewModel>()
        where TViewModel : class, ISupportCancellation
    {
        ISupportCancellation viewModel = ServiceProvider.GetRequiredService<TViewModel>();
        viewModel.CancellationToken = viewLoadingCancellationTokenSource.Token;
        DataContext = viewModel;
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        viewLoadingCancellationTokenSource.Cancel();

        // Try dispose scope when page is not presented
        serviceScope.Dispose();
    }
}