// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Navigation.Message;
using Snap.Hutao.UI.Content;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.View.Page;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Behavior;

internal sealed class ServiceRecipientNavigationViewBehavior : BehaviorBase<NavigationView>,
    IRecipient<NavigationNavigateMessage>,
    IRecipient<NavigationGoBackMessage>,
    IRecipient<NavigationPaneToggleMessage>
{
    private IMessenger? messenger;

    public void Receive(NavigationNavigateMessage message)
    {
        if (AssociatedObject is not { } navigationView)
        {
            message.Result = NavigationResult.Failed;
            return;
        }

        message.Result = Navigate(navigationView, message.PageType, message.Data, message.SyncNavigationViewItem);
    }

    public void Receive(NavigationGoBackMessage message)
    {
        if (AssociatedObject is not { } navigationView || navigationView.Content.As<Frame>() is not { } frame)
        {
            message.Result = false;
            return;
        }

        if (frame.CanGoBack)
        {
            frame.GoBack();
            message.Result = SyncSelectedNavigationViewItem(navigationView, frame.Content?.GetType());
        }
    }

    public void Receive(NavigationPaneToggleMessage message)
    {
        if (AssociatedObject is not { } navigationView)
        {
            return;
        }

        navigationView.IsPaneOpen = !navigationView.IsPaneOpen;
    }

    protected override void OnAssociatedObjectLoaded()
    {
        AssociatedObject.IsPaneOpen = LocalSetting.Get(SettingKeys.IsNavPaneOpen, true);

        if (AssociatedObject.XamlRoot.XamlContext()?.ServiceProvider is { } serviceProvider)
        {
            messenger = serviceProvider.GetRequiredService<IMessenger>();
            messenger.Register<NavigationNavigateMessage>(this);
            messenger.Register<NavigationGoBackMessage>(this);
            messenger.Register<NavigationPaneToggleMessage>(this);
        }

        AssociatedObject.ItemInvoked += OnItemInvoked;
        AssociatedObject.PaneOpened += OnPaneStateChanged;
        AssociatedObject.PaneClosed += OnPaneStateChanged;

        Navigate(AssociatedObject, typeof(AnnouncementPage), NavigationExtraData.Default, true);
    }

    protected override bool Uninitialize()
    {
        messenger?.UnregisterAll(this);
        AssociatedObject.PaneOpened -= OnPaneStateChanged;
        AssociatedObject.PaneClosed -= OnPaneStateChanged;
        return base.Uninitialize();
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

    private static void OnPaneStateChanged(NavigationView sender, object args)
    {
        ArgumentNullException.ThrowIfNull(sender);
        LocalSetting.Set(SettingKeys.IsNavPaneOpen, sender.IsPaneOpen);
    }

    private static void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is not NavigationViewItem item)
        {
            return;
        }

        Type? targetType = args.IsSettingsInvoked
            ? typeof(SettingPage)
            : NavigationViewItemHelper.GetNavigateTo(item);

        // Ignore item that doesn't have nav type specified
        if (targetType is not null)
        {
            INavigationCompletionSource data = new NavigationExtraData(NavigationViewItemHelper.GetExtraData(item));
            Navigate(sender, targetType, data, syncNavigationViewItem: false); // Because we are already invoking the item
        }
    }

    private static NavigationResult Navigate(NavigationView navigationView, Type pageType, INavigationCompletionSource data, bool syncNavigationViewItem = false)
    {
        if (navigationView.Content.As<Frame>() is not { } frame)
        {
            return NavigationResult.Failed;
        }

        Page? currentPage = frame.Content?.As<Page>();
        Type? currentPageType = currentPage?.GetType();

        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateNavigation(
            currentPageType is null ? "Empty" : TypeNameHelper.GetTypeDisplayName(currentPageType, fullName: false),
            TypeNameHelper.GetTypeDisplayName(pageType, fullName: false),
            "Navigation"));

        if (currentPageType == pageType)
        {
            CancellationToken token = currentPage is ScopedPage scopedPage ? scopedPage.CancellationToken : CancellationToken.None;
            NavigationExtraDataSupport.NotifyRecipientAsync(frame.Content, data, token).SafeForget();
            return NavigationResult.AlreadyNavigatedTo;
        }

        _ = syncNavigationViewItem && SyncSelectedNavigationViewItem(navigationView, pageType);

        bool navigated = false;
        try
        {
            navigated = data is ISupportNavigationTransitionInfo { TransitionInfo: { } transitionInfo }
                ? frame.Navigate(pageType, data, transitionInfo)
                : frame.Navigate(pageType, data);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }

        return navigated ? NavigationResult.Succeed : NavigationResult.Failed;
    }

    private static bool SyncSelectedNavigationViewItem(NavigationView navigationView, Type? pageType)
    {
        if (pageType is null)
        {
            return false;
        }

        if (pageType == typeof(SettingPage))
        {
            navigationView.SelectedItem = navigationView.SettingsItem;
            return true;
        }

        NavigationViewItem? target = EnumerateMenuItems(navigationView.MenuItems)
            .SingleOrDefault(menuItem => NavigationViewItemHelper.GetNavigateTo(menuItem) == pageType);

        if (target is null)
        {
            return false;
        }

        navigationView.SelectedItem = target;
        return true;
    }
}