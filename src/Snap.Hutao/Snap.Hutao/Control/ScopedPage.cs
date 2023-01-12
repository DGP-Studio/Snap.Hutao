// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.ViewModel.Abstraction;

namespace Snap.Hutao.Control;

/// <summary>
/// 表示支持取消加载的异步页面
/// 在被导航到其他页面前触发取消异步通知
/// </summary>
[SuppressMessage("", "CA1001")]
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
        serviceScope.Track();
    }

    /// <summary>
    /// 初始化
    /// 应当在 InitializeComponent() 前调用
    /// </summary>
    /// <typeparam name="TViewModel">视图模型类型</typeparam>
    public void InitializeWith<TViewModel>()
        where TViewModel : class, IViewModel
    {
        IViewModel viewModel = serviceScope.ServiceProvider.GetRequiredService<TViewModel>();
        viewModel.CancellationToken = viewLoadingCancellationTokenSource.Token;
        DataContext = viewModel;
    }

    /// <summary>
    /// 异步通知接收器
    /// </summary>
    /// <param name="extra">额外内容</param>
    /// <returns>任务</returns>
    public async Task NotifyRecipentAsync(INavigationData extra)
    {
        if (extra.Data != null && DataContext is INavigationRecipient recipient)
        {
            await recipient.ReceiveAsync(extra).ConfigureAwait(false);
        }

        extra.NotifyNavigationCompleted();
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        using (viewLoadingCancellationTokenSource)
        {
            // Cancel tasks executed by the view model
            viewLoadingCancellationTokenSource.Cancel();
            IViewModel viewModel = (IViewModel)DataContext;

            using (SemaphoreSlim locker = viewModel.DisposeLock)
            {
                // Wait to ensure viewmodel operation is completed
                locker.Wait();
                viewModel.IsViewDisposed = true;

                // Dispose the scope
                serviceScope.Dispose();
            }
        }
    }

    /// <inheritdoc/>
    [SuppressMessage("", "VSTHRD100")]
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is INavigationData extra)
        {
            await NotifyRecipentAsync(extra).ConfigureAwait(false);
        }
    }
}