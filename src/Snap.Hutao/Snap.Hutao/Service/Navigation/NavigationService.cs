// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.View.Helper;
using Snap.Hutao.View.Page;
using Windows.Foundation;

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航服务
/// </summary>
[HighQuality]
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
    private NavigationView? navigationView;
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

    public Type? Current { get => frame?.Content.GetType(); }

    private NavigationView? NavigationView
    {
        get => navigationView;

        set
        {
            // remove old listener
            if (navigationView is not null)
            {
                navigationView.ItemInvoked -= itemInvokedEventHandler;
                navigationView.BackRequested -= backRequestedEventHandler;
                navigationView.PaneClosed -= paneOpenedEventHandler;
                navigationView.PaneOpened -= paneClosedEventHandler;
            }

            ArgumentNullException.ThrowIfNull(value);
            navigationView = value;

            // add new listener
            if (navigationView is not null)
            {
                navigationView.ItemInvoked += itemInvokedEventHandler;
                navigationView.BackRequested += backRequestedEventHandler;
                navigationView.PaneClosed += paneOpenedEventHandler;
                navigationView.PaneOpened += paneClosedEventHandler;
            }
        }
    }

    /// <inheritdoc/>
    public NavigationResult Navigate(Type pageType, INavigationAwaiter data, bool syncNavigationViewItem = false)
    {
        Type? currentType = frame?.Content?.GetType();

        if (currentType == pageType)
        {
            logger.LogInformation("Navigate to {pageType} : succeed, already in", pageType);
            return NavigationResult.AlreadyNavigatedTo;
        }

        _ = syncNavigationViewItem && SyncSelectedNavigationViewItemWith(pageType);

        bool navigated = false;
        try
        {
            navigated = frame?.Navigate(pageType, data) ?? false;
            logger.LogInformation("Navigate to {pageType} : {result}", pageType, navigated ? "succeed" : "failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while navigating to {pageType}", pageType);
            infoBarService.Error(ex);
        }

        return navigated ? NavigationResult.Succeed : NavigationResult.Failed;
    }

    /// <inheritdoc/>
    public NavigationResult Navigate<TPage>(INavigationAwaiter data, bool syncNavigationViewItem = false)
        where TPage : Page
    {
        return Navigate(typeof(TPage), data, syncNavigationViewItem);
    }

    /// <inheritdoc/>
    public async ValueTask<NavigationResult> NavigateAsync<TPage>(INavigationAwaiter data, bool syncNavigationViewItem = false)
        where TPage : Page
    {
        NavigationResult result = Navigate<TPage>(data, syncNavigationViewItem);

        switch (result)
        {
            case NavigationResult.Succeed:
                {
                    try
                    {
                        await data.WaitForCompletionAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "异步导航时发生异常");
                        return NavigationResult.Failed;
                    }
                }

                break;

            case NavigationResult.AlreadyNavigatedTo:
                {
                    if (frame is { Content: ScopedPage scopedPage })
                    {
                        await scopedPage.NotifyRecipientAsync((INavigationData)data).ConfigureAwait(false);
                    }
                }

                break;
        }

        return result;
    }

    /// <inheritdoc/>
    public void Initialize(NavigationView navigationView, Frame frame)
    {
        NavigationView = navigationView;
        this.frame = frame;

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
                .SingleOrDefault(menuItem => NavHelper.GetNavigateTo(menuItem) == pageType);

            NavigationView.SelectedItem = target;
        }

        selected = NavigationView.SelectedItem as NavigationViewItem;
        return true;
    }

    private IEnumerable<NavigationViewItem> EnumerateMenuItems(IList<object> items)
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

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        selected = NavigationView?.SelectedItem as NavigationViewItem;
        Type? targetType = args.IsSettingsInvoked
            ? typeof(SettingPage)
            : NavHelper.GetNavigateTo(selected);

        // ignore item that doesn't have nav type specified
        if (targetType is not null)
        {
            INavigationAwaiter navigationAwaiter = new NavigationExtra(NavHelper.GetExtraData(selected));
            Navigate(targetType, navigationAwaiter, false);
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