// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.UI.Content;
using Snap.Hutao.ViewModel.Abstraction;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "CA1001")]
internal partial class ScopedPage : Page
{
    private readonly CancellationTokenSource viewCts = new();
    private IServiceScope? scope;

    protected ScopedPage()
    {
        Loading += OnLoading;
        Unloaded += OnUnloaded;
    }

    public virtual void UnloadObjectOverride(DependencyObject unloadableObject)
    {
        XamlMarkupHelper.UnloadObject(unloadableObject);
    }

    protected virtual void LoadingOverride()
    {
    }

    protected void InitializeWith<TViewModel>()
        where TViewModel : class, IViewModel
    {
        ArgumentNullException.ThrowIfNull(scope);
        TViewModel viewModel = scope.ServiceProvider.GetRequiredService<TViewModel>();
        using (viewModel.DisposeLock.Enter())
        {
            viewModel.Resurrect();
            viewModel.CancellationToken = viewCts.Token;
            viewModel.DeferContentLoader = new DeferContentLoader(this);
        }

        DataContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is INavigationCompletionSource data)
        {
            NavigationExtraDataSupport
                .NotifyRecipientAsync(this, data)
                .SafeForget();
        }
    }

    private void OnLoading(FrameworkElement element, object e)
    {
        Loading -= OnLoading;
        scope = element.XamlRoot.XamlContext().ServiceProvider.CreateScope();
        LoadingOverride();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Cancel all tasks executed by the view model
        viewCts.Cancel();
        IViewModel viewModel = (IViewModel)DataContext;

        // Wait to ensure viewmodel operation is completed
        using (viewModel.DisposeLock.Enter())
        {
            viewModel.Uninitialize();
        }

        viewCts.Dispose();

        // Dispose the scope
        scope?.Dispose();
        scope = default;

        if (this.IsDisposed())
        {
            return;
        }

        Unloaded -= OnUnloaded;
    }
}