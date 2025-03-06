// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.View.Page;
using Windows.Foundation;

namespace Snap.Hutao.Service.Navigation;

[Injection(InjectAs.Singleton, typeof(INavigationService))]
internal sealed class NavigationService : INavigationService, INavigationInitialization
{
    private readonly ILogger<INavigationService> logger;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private readonly TypedEventHandler<NavigationView, NavigationViewItemInvokedEventArgs> itemInvokedEventHandler;
    private readonly TypedEventHandler<NavigationView, NavigationViewBackRequestedEventArgs> backRequestedEventHandler;
    private readonly TypedEventHandler<NavigationView, object> paneOpenedEventHandler;
    private readonly TypedEventHandler<NavigationView, object> paneClosedEventHandler;

    private Frame? frame;
    private NavigationViewItem? selected;

    public NavigationService(IServiceProvider serviceProvider)
    {
        logger = serviceProvider.GetRequiredService<ILogger<INavigationService>>();
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        itemInvokedEventHandler = OnItemInvoked;
        backRequestedEventHandler = OnBackRequested;
        paneOpenedEventHandler = OnPaneStateChanged;
        paneClosedEventHandler = OnPaneStateChanged;
    }

    public Type? CurrentPageType { get => frame?.Content?.GetType(); }

    [DisallowNull]
    private NavigationView? NavigationView
    {
        get;

        set
        {
            // remove old listener
            if (field is not null)
            {
                field.ItemInvoked -= itemInvokedEventHandler;
                field.BackRequested -= backRequestedEventHandler;
                field.PaneClosed -= paneOpenedEventHandler;
                field.PaneOpened -= paneClosedEventHandler;
            }

            field = value;

            // add new listener
            if (field is not null)
            {
                field.ItemInvoked += itemInvokedEventHandler;
                field.BackRequested += backRequestedEventHandler;
                field.PaneClosed += paneOpenedEventHandler;
                field.PaneOpened += paneClosedEventHandler;
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

        if (CurrentPageType == pageType)
        {
            NavigationExtraDataSupport.NotifyRecipientAsync(frame?.Content, data).SafeForget(logger);
            return NavigationResult.AlreadyNavigatedTo;
        }

        _ = syncNavigationViewItem && SyncSelectedNavigationViewItemWith(pageType);

        bool navigated = false;
        try
        {
            navigated = frame?.Navigate(pageType, data) ?? false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            infoBarService.Error(ex);
        }

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
                logger.LogError(ex, "An error occurred while waiting for the navigation to the \e[1m\e[36m{pageType} to complete\e[37m", typeof(TPage));
                return NavigationResult.Failed;
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public void Initialize(INavigationViewAccessor accessor)
    {
        NavigationView = accessor.NavigationView;
        frame = accessor.Frame;

        NavigationView.IsPaneOpen = LocalSetting.Get(SettingKeys.IsNavPaneOpen, true);
    }

    /// <inheritdoc/>
    public void GoBack()
    {
        taskContext.InvokeOnMainThread(() =>
        {
            if (frame is { CanGoBack: true })
            {
                frame.GoBack();
                SyncSelectedNavigationViewItemWith(frame.Content.GetType());
            }
        });
    }

    private static IEnumerable<NavigationViewItem> EnumerateMenuItems(IList<object> items)
    {
        foreach (NavigationViewItem item in items.OfType<NavigationViewItem>())
        {
            yield return item;

            // Suppress recursion method call if possible
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

        selected = NavigationView.SelectedItem as NavigationViewItem;
        return true;
    }

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        selected = NavigationView?.SelectedItem as NavigationViewItem;
        Type? targetType = args.IsSettingsInvoked
            ? typeof(SettingPage)
            : NavigationViewItemHelper.GetNavigateTo(selected);

        // ignore item that doesn't have nav type specified
        if (targetType is not null)
        {
            INavigationCompletionSource data = new NavigationCompletionSource(NavigationViewItemHelper.GetExtraData(selected));
            Navigate(targetType, data, false);
        }
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (frame is { CanGoBack: true })
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
}