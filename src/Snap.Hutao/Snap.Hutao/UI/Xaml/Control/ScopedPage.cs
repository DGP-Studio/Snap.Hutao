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
        // Events/Override Methods order
        // ----------------------------------------------------------------------
        // Page Navigation methods:
        // - OnNavigatedTo
        // FrameworkElement events:
        // - Loading (args: null)
        // - EffectiveViewportChanged
        // - SizeChanged
        // - LayoutUpdated (sender & args always null)
        // - Loaded
        // - LayoutUpdated (sender & args always null) (Can trigger multiple times)
        // OnNavigatedTo -> Loading -> Loaded
        // ----------------------------------------------------------------------
        // Page Navigation methods:
        // - OnNavigatingFrom
        // - OnNavigatedFrom
        // FrameworkElement events:
        // - LayoutUpdated (sender & args always null)
        // - Unloaded
        // - LayoutUpdated (might or might not be called)
        // OnNavigatingFrom -> OnNavigatedFrom -> Unloaded
        // ----------------------------------------------------------------------
        Loading += OnLoading;
        Unloaded += OnUnloaded;
    }

    public CancellationToken CancellationToken { get => viewCts.Token; }

    public virtual void UnloadObjectOverride(DependencyObject unloadableObject)
    {
        XamlMarkupHelper.UnloadObject(unloadableObject);
    }

    /// <summary>
    /// Override this method to implement the loading logic.
    /// The page is not attached to the visual tree yet when this method is called.
    /// </summary>
    protected virtual void LoadingOverride()
    {
    }

    /// <summary>
    /// Set <see cref="FrameworkElement.DataContext"/> to an instance of <typeparamref name="TViewModel"/>, which will be retrieved from ServiceProvider
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel</typeparam>
    protected void InitializeDataContext<TViewModel>()
        where TViewModel : class, IViewModel
    {
        ArgumentNullException.ThrowIfNull(scope);

        TViewModel viewModel = scope.ServiceProvider.GetRequiredService<TViewModel>();
        using (viewModel.DisposeLock.Enter())
        {
            viewModel.Resurrect();
            viewModel.CancellationToken = CancellationToken;
            viewModel.DeferContentLoader = new DeferContentLoader(this);

            DataContext = viewModel;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        // OnNavigatedTo is called before any FrameworkElement event on the page.
        if (e.Parameter is INavigationCompletionSource data)
        {
            NavigationExtraDataSupport.NotifyRecipientAsync(this, data, CancellationToken).SafeForget();
        }
    }

    private void OnLoading(FrameworkElement element, object e)
    {
        Loading -= OnLoading;

        XamlContext? context = element.XamlRoot.XamlContext();
        ArgumentNullException.ThrowIfNull(context);
        scope = context.ServiceProvider.CreateScope();

        LoadingOverride();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Cancel all tasks executed by the view model
        viewCts.Cancel();

        if (DataContext is IViewModel viewModel)
        {
            // Wait to ensure viewmodel operation is completed
            using (viewModel.DisposeLock.Enter())
            {
                viewModel.Uninitialize();
            }
        }

        DataContext = default;

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