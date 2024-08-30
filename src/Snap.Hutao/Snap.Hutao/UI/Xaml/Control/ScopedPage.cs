// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.ViewModel.Abstraction;

namespace Snap.Hutao.UI.Xaml.Control;

[HighQuality]
[SuppressMessage("", "CA1001")]
internal partial class ScopedPage : Page
{
    private readonly CancellationTokenSource viewCancellationTokenSource = new();
    private readonly IServiceScope pageScope;

    private bool inFrame = true;

    protected ScopedPage()
    {
        Unloaded += OnUnloaded;
        pageScope = Ioc.Default.GetRequiredService<IScopedPageScopeReferenceTracker>().CreateScope();
    }

    [SuppressMessage("", "SH003")]
    public async Task NotifyRecipientAsync(INavigationData extra)
    {
        if (extra.Data is not null && DataContext is INavigationRecipient recipient)
        {
            await recipient.ReceiveAsync(extra).ConfigureAwait(false);
        }

        extra.NotifyNavigationCompleted();
    }

    public virtual void UnloadObjectOverride(DependencyObject unloadableObject)
    {
        XamlMarkupHelper.UnloadObject(unloadableObject);
    }

    /// <summary>
    /// 初始化
    /// 应当在 InitializeComponent() 前调用
    /// </summary>
    /// <typeparam name="TViewModel">视图模型类型</typeparam>
    protected void InitializeWith<TViewModel>()
        where TViewModel : class, IViewModel
    {
        try
        {
            TViewModel viewModel = pageScope.ServiceProvider.GetRequiredService<TViewModel>();
            using (viewModel.DisposeLock.Enter())
            {
                viewModel.Resurrect();
                viewModel.CancellationToken = viewCancellationTokenSource.Token;
                viewModel.DeferContentLoader = new DeferContentLoader(this);
            }

            DataContext = viewModel;
        }
        catch (Exception ex)
        {
            pageScope.ServiceProvider.GetRequiredService<ILogger<ScopedPage>>().LogError(ex, "Failed to initialize view model.");
            throw;
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        DisposeViewModel();
        inFrame = false;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is INavigationData extra)
        {
            _ = NotifyRecipientAsync(extra);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (inFrame)
        {
            DisposeViewModel();
        }

        if (this.IsDisposed())
        {
            return;
        }

        Unloaded -= OnUnloaded;
    }

    private void DisposeViewModel()
    {
        using (viewCancellationTokenSource)
        {
            // Cancel all tasks executed by the view model
            viewCancellationTokenSource.Cancel();
            IViewModel viewModel = (IViewModel)DataContext;

            // Wait to ensure viewmodel operation is completed
            using (viewModel.DisposeLock.Enter())
            {
                viewModel.Uninitialize();

                // Dispose the scope
                pageScope.Dispose();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true);
            }
        }
    }
}