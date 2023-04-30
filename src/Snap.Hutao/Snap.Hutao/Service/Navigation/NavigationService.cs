// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.View.Helper;
using Snap.Hutao.View.Page;

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(INavigationService))]
internal sealed class NavigationService : INavigationService
{
    private readonly ITaskContext taskContext;
    private readonly IInfoBarService infoBarService;
    private readonly ILogger<INavigationService> logger;

    private NavigationView? navigationView;

    /// <summary>
    /// 构造一个新的导航服务
    /// </summary>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="logger">日志器</param>
    public NavigationService(ITaskContext taskContext, IInfoBarService infoBarService, ILogger<INavigationService> logger)
    {
        this.taskContext = taskContext;
        this.infoBarService = infoBarService;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public Frame? Frame { get; set; }

    /// <inheritdoc/>
    public NavigationView? NavigationView
    {
        get => navigationView;

        set
        {
            // remove old listener
            if (navigationView != null)
            {
                navigationView.ItemInvoked -= OnItemInvoked;
                navigationView.BackRequested -= OnBackRequested;
                navigationView.PaneClosed -= OnPaneStateChanged;
                navigationView.PaneOpened -= OnPaneStateChanged;
            }

            navigationView = Must.NotNull(value!);

            // add new listener
            if (navigationView != null)
            {
                navigationView.ItemInvoked += OnItemInvoked;
                navigationView.BackRequested += OnBackRequested;
                navigationView.PaneClosed += OnPaneStateChanged;
                navigationView.PaneOpened += OnPaneStateChanged;
            }
        }
    }

    /// <inheritdoc/>
    public NavigationViewItem? Selected { get; set; }

    /// <inheritdoc/>
    public bool HasEverNavigated { get; set; }

    /// <inheritdoc/>
    public bool SyncSelectedNavigationViewItemWith(Type? pageType)
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

        Selected = NavigationView.SelectedItem as NavigationViewItem;
        return true;
    }

    /// <inheritdoc/>
    public NavigationResult Navigate(Type pageType, INavigationAwaiter data, bool syncNavigationViewItem = false)
    {
        Type? currentType = Frame?.Content?.GetType();

        if (currentType == pageType)
        {
            logger.LogInformation("Navigate to {pageType} : succeed, already in", pageType);
            return NavigationResult.AlreadyNavigatedTo;
        }

        _ = syncNavigationViewItem && SyncSelectedNavigationViewItemWith(pageType);

        bool navigated = false;
        try
        {
            navigated = Frame?.Navigate(pageType, data) ?? false;
            logger.LogInformation("Navigate to {pageType} : {result}", pageType, navigated ? "succeed" : "failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while navigating to {pageType}", pageType);
            infoBarService.Error(ex);
        }

        // 首次导航失败时使属性持续保存为false
        HasEverNavigated = HasEverNavigated || navigated;
        return navigated ? NavigationResult.Succeed : NavigationResult.Failed;
    }

    /// <inheritdoc/>
    public NavigationResult Navigate<TPage>(INavigationAwaiter data, bool syncNavigationViewItem = false)
        where TPage : Page
    {
        return Navigate(typeof(TPage), data, syncNavigationViewItem);
    }

    /// <inheritdoc/>
    public async Task<NavigationResult> NavigateAsync<TPage>(INavigationAwaiter data, bool syncNavigationViewItem = false)
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
                    if (Frame!.Content is ScopedPage scopedPage)
                    {
                        await scopedPage.NotifyRecipentAsync((INavigationData)data).ConfigureAwait(false);
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
        Frame = frame;

        NavigationView.IsPaneOpen = LocalSetting.Get(SettingKeys.IsNavPaneOpen, true);
    }

    /// <inheritdoc/>
    public void GoBack()
    {
        taskContext.InvokeOnMainThread(() =>
        {
            bool canGoBack = Frame?.CanGoBack ?? false;

            if (canGoBack)
            {
                Frame!.GoBack();
                SyncSelectedNavigationViewItemWith(Frame.Content.GetType());
            }
        });
    }

    /// <summary>
    /// 遍历所有子菜单项
    /// </summary>
    /// <param name="items">项列表</param>
    /// <returns>枚举器</returns>
    private IEnumerable<NavigationViewItem> EnumerateMenuItems(IList<object> items)
    {
        foreach (NavigationViewItem item in items.OfType<NavigationViewItem>())
        {
            yield return item;

            foreach (NavigationViewItem subItem in EnumerateMenuItems(item.MenuItems))
            {
                yield return subItem;
            }
        }
    }

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        Selected = NavigationView?.SelectedItem as NavigationViewItem;
        Type? targetType = args.IsSettingsInvoked
            ? typeof(SettingPage)
            : NavHelper.GetNavigateTo(Selected);

        // ignore item that doesn't have nav type specified
        if (targetType != null)
        {
            INavigationAwaiter navigationAwaiter = new NavigationExtra(NavHelper.GetExtraData(Selected));
            Navigate(targetType, navigationAwaiter, false);
        }
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (Frame != null && Frame.CanGoBack)
        {
            Frame.GoBack();
            SyncSelectedNavigationViewItemWith(Frame.Content.GetType());
        }
    }

    private void OnPaneStateChanged(NavigationView sender, object args)
    {
        LocalSetting.Set(SettingKeys.IsNavPaneOpen, NavigationView!.IsPaneOpen);
    }
}