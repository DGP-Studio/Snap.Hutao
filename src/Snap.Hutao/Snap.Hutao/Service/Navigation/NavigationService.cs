// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.View.Page;

namespace Snap.Hutao.Service.Navigation;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(INavigationService))]
internal sealed partial class NavigationService : INavigationService, INavigationInitialization
{
    private readonly ILogger<INavigationService> logger;
    private readonly ITaskContext taskContext;

    private readonly WeakReference<NavigationView> weakNavigationView = new(default!);
    private readonly WeakReference<Frame> weakFrame = new(default!);
    private readonly WeakReference<NavigationViewItemBase?> weakItem = new(default!);

    public Type? CurrentPageType { get => weakFrame.TryGetTarget(out Frame? frame) ? frame.Content?.GetType() : default; }

    private NavigationView? NavigationView
    {
        get => weakNavigationView.TryGetTarget(out NavigationView? navigationView) ? navigationView : null;

        set
        {
            // remove old listener
            if (weakNavigationView.TryGetTarget(out NavigationView? oldValue))
            {
                oldValue.ItemInvoked -= OnItemInvoked;
                oldValue.BackRequested -= OnBackRequested;
                oldValue.PaneClosed -= OnPaneStateChanged;
                oldValue.PaneOpened -= OnPaneStateChanged;
                oldValue.Unloaded -= OnUnloaded;
            }

            weakNavigationView.SetTarget(value!);

            if (weakNavigationView.TryGetTarget(out NavigationView? newValue))
            {
                newValue.ItemInvoked += OnItemInvoked;
                newValue.BackRequested += OnBackRequested;
                newValue.PaneClosed += OnPaneStateChanged;
                newValue.PaneOpened += OnPaneStateChanged;
                newValue.Unloaded += OnUnloaded;
            }
        }
    }

    /// <inheritdoc/>
    public NavigationResult Navigate(Type pageType, INavigationCompletionSource data, bool syncNavigationViewItem = false)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateNavigation(
            CurrentPageType is null ? "Empty" : TypeNameHelper.GetTypeDisplayName(CurrentPageType, fullName: false),
            TypeNameHelper.GetTypeDisplayName(pageType, fullName: false),
            "Navigation"));

        Verify.Operation(weakFrame.TryGetTarget(out Frame? frame), "NavigationService not initialized, no target frame set");

        if (CurrentPageType == pageType)
        {
            NavigationExtraDataSupport.NotifyRecipientAsync(frame.Content, data).SafeForget(logger);
            return NavigationResult.AlreadyNavigatedTo;
        }

        _ = syncNavigationViewItem && SyncSelectedNavigationViewItemWith(pageType);

        bool navigated = false;
        try
        {
            navigated = frame.Navigate(pageType, data);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }

        GC.KeepAlive(frame);
        return navigated ? NavigationResult.Succeed : NavigationResult.Failed;
    }

    /// <inheritdoc/>
    public NavigationResult Navigate<TPage>(INavigationCompletionSource data, bool syncNavigationViewItem = false)
        where TPage : Page
    {
        return Navigate(typeof(TPage), data, syncNavigationViewItem);
    }

    /// <inheritdoc/>
    public async ValueTask<NavigationResult> NavigateAsync<TPage>(INavigationCompletionSource data, bool syncNavigationViewItem = false)
        where TPage : Page
    {
        await taskContext.SwitchToMainThreadAsync();
        NavigationResult result = Navigate<TPage>(data, syncNavigationViewItem);

        if (result is NavigationResult.Succeed)
        {
            try
            {
                await taskContext.SwitchToBackgroundAsync();
                await data.WaitForCompletionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return NavigationResult.Failed;
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public void Initialize(INavigationViewAccessor accessor)
    {
        NavigationView = accessor.NavigationView;
        weakFrame.SetTarget(accessor.Frame);

        NavigationView.IsPaneOpen = LocalSetting.Get(SettingKeys.IsNavPaneOpen, true);
    }

    /// <inheritdoc/>
    public void GoBack()
    {
        taskContext.InvokeOnMainThread(() =>
        {
            if (weakFrame.TryGetTarget(out Frame? frame))
            {
                if (frame is { CanGoBack: true })
                {
                    frame.GoBack();
                    SyncSelectedNavigationViewItemWith(frame.Content.GetType());
                }
            }
        });
    }

    private static IEnumerable<NavigationViewItem> EnumerateMenuItems(IList<object> items)
    {
        foreach (NavigationViewItem item in items.OfType<NavigationViewItem>())
        {
            yield return item;

            if (item.MenuItems.Count > 0)
            {
                foreach (NavigationViewItem subItem in EnumerateMenuItems(item.MenuItems))
                {
                    yield return subItem;
                }
            }
        }
    }

    private bool SyncSelectedNavigationViewItemWith(Type? pageType)
    {
        if (NavigationView is null || pageType is null)
        {
            return false;
        }

        if (pageType == typeof(SettingPage))
        {
            NavigationView.SelectedItem = NavigationView.SettingsItem;
        }
        else
        {
            NavigationViewItem? target = EnumerateMenuItems(NavigationView.MenuItems)
                .SingleOrDefault(menuItem => NavigationViewItemHelper.GetNavigateTo(menuItem) == pageType);

            NavigationView.SelectedItem = target;
        }

        weakItem.SetTarget(NavigationView.SelectedItem as NavigationViewItem);
        return true;
    }

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        weakItem.SetTarget(args.InvokedItemContainer);
        NavigationViewItem? item = args.InvokedItemContainer as NavigationViewItem;
        Type? targetType = args.IsSettingsInvoked
            ? typeof(SettingPage)
            : NavigationViewItemHelper.GetNavigateTo(item);

        // Ignore item that doesn't have nav type specified
        if (targetType is not null)
        {
            INavigationCompletionSource data = new NavigationCompletionSource(NavigationViewItemHelper.GetExtraData(item));
            Navigate(targetType, data, syncNavigationViewItem: false); // Because we are already invoking the item
        }
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (weakFrame.TryGetTarget(out Frame? frame) && frame is { CanGoBack: true })
        {
            frame.GoBack();
            SyncSelectedNavigationViewItemWith(frame.Content.GetType());
        }
    }

    private void OnPaneStateChanged(NavigationView sender, object args)
    {
        ArgumentNullException.ThrowIfNull(NavigationView);
        LocalSetting.Set(SettingKeys.IsNavPaneOpen, NavigationView.IsPaneOpen);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        NavigationView = default;
    }
}