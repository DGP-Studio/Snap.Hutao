// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Abstraction.Navigation;
using Snap.Hutao.View.Helper;
using Snap.Hutao.View.Page;
using System.Linq;

namespace Snap.Hutao.Service;

/// <summary>
/// 导航服务
/// </summary>
[Injection(InjectAs.Singleton, typeof(INavigationService))]
internal class NavigationService : INavigationService
{
    private readonly IInfoBarService infoBarService;
    private readonly ILogger<INavigationService> logger;

    private NavigationView? navigationView;

    /// <summary>
    /// 构造一个新的导航服务
    /// </summary>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="logger">日志器</param>
    public NavigationService(IInfoBarService infoBarService, ILogger<INavigationService> logger)
    {
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
            }

            navigationView = Must.NotNull(value!, "NavigationView");

            // add new listener
            if (navigationView != null)
            {
                navigationView.ItemInvoked += OnItemInvoked;
                navigationView.BackRequested += OnBackRequested;
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
            NavigationViewItem? target = NavigationView.MenuItems
                .OfType<NavigationViewItem>()
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
            return NavigationResult.AlreadyNavigatedTo;
        }

        _ = syncNavigationViewItem && SyncSelectedNavigationViewItemWith(pageType);

        bool navigated = false;
        try
        {
            navigated = Frame?.Navigate(pageType, data) ?? false;
        }
        catch (Exception ex)
        {
            logger.LogError(EventIds.NavigationFailed, ex, "导航到指定页面时发生了错误");
            infoBarService.Error(ex);
        }

        logger.LogInformation("Navigate to {pageType}:{result}", pageType, navigated ? "succeed" : "failed");

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

        if (result is NavigationResult.Succeed)
        {
            try
            {
                await data.WaitForCompletionAsync();
            }
            catch (AggregateException)
            {
                return NavigationResult.Failed;
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public void Initialize(NavigationView navigationView, Frame frame)
    {
        NavigationView = navigationView;
        Frame = frame;
    }

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        Selected = NavigationView?.SelectedItem as NavigationViewItem;
        Type? targetType = args.IsSettingsInvoked
            ? typeof(SettingPage)
            : NavHelper.GetNavigateTo(Selected);

        INavigationAwaiter navigationAwaiter = new NavigationExtra(NavHelper.GetExtraData(Selected));
        Navigate(Must.NotNull(targetType!), navigationAwaiter, false);
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (Frame != null && Frame.CanGoBack)
        {
            Frame.GoBack();
            SyncSelectedNavigationViewItemWith(Frame.Content.GetType());
        }
    }
}