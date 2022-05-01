// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.AppCenter.Analytics;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.View.Helper;
using Snap.Hutao.View.Page;
using System.Linq;

namespace Snap.Hutao.Service;

/// <summary>
/// 导航服务
/// </summary>
[Injection(InjectAs.Transient, typeof(INavigationService))]
internal class NavigationService : INavigationService
{
    private readonly ILogger<INavigationService> logger;

    private NavigationView? navigationView;

    /// <summary>
    /// 构造一个新的导航服务
    /// </summary>
    /// <param name="logger">日志器</param>
    public NavigationService(ILogger<INavigationService> logger)
    {
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
    public bool SyncTabWith(Type pageType)
    {
        if (NavigationView is null)
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
    public bool Navigate(Type? pageType, bool isSyncTabRequested = false, object? data = null)
    {
        Type? currentType = Frame?.Content?.GetType();
        if (pageType is null || (currentType == pageType))
        {
            return false;
        }

        _ = isSyncTabRequested && SyncTabWith(pageType);

        bool result = false;
        try
        {
            result = Frame?.Navigate(pageType, data) ?? false;
        }
        catch (Exception ex)
        {
            logger.LogError(EventIds.NavigationFailed, ex, "导航到指定页面时发生了错误");
        }

        logger.LogInformation("Navigate to {pageType}:{result}", pageType, result ? "succeed" : "failed");

        // 分析页面统计数据时不应加入启动时导航的首个页面
        if (HasEverNavigated)
        {
            Analytics.TrackEvent("General", ("OpenUI", pageType.ToString()).AsDictionary());
        }

        // 首次导航失败时使属性持续保存为false
        HasEverNavigated |= result;
        return result;
    }

    /// <inheritdoc/>
    public bool Navigate<T>(bool isSyncTabRequested = false, object? data = null)
        where T : Page
    {
        return Navigate(typeof(T), isSyncTabRequested, data);
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
        Navigate(targetType, false, NavHelper.GetExtraData(Selected));
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (Frame != null && Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }
}
