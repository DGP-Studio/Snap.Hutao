// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航服务
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 管理的 <see cref="Frame"/>
    /// </summary>
    Frame? Frame { get; set; }

    /// <summary>
    /// 指示是否曾导航过,用于启动时导航判断
    /// </summary>
    bool HasEverNavigated { get; set; }

    /// <summary>
    /// 管理的 <see cref="NavigationView"/>
    /// </summary>
    NavigationView? NavigationView { get; set; }

    /// <summary>
    /// 选中的 <see cref="NavigationViewItem"/>
    /// </summary>
    NavigationViewItem? Selected { get; set; }

    /// <summary>
    /// 使用指定的对象进行初始化
    /// </summary>
    /// <param name="navigationView">管理的 <see cref="NavigationView"/></param>
    /// <param name="frame">管理的 <see cref="Frame"/></param>
    void Initialize(NavigationView navigationView, Frame frame);

    /// <summary>
    /// 导航到指定类型的页面
    /// </summary>
    /// <param name="pageType">指定的页面类型</param>
    /// <param name="data">要传递的数据</param>
    /// <param name="isSyncTabRequested">是否同步标签，当在代码中调用时应设为 true</param>
    /// <returns>是否导航成功</returns>
    NavigationResult Navigate(Type pageType, INavigationAwaiter data, bool isSyncTabRequested = false);

    /// <summary>
    /// 导航到指定类型的页面
    /// </summary>
    /// <typeparam name="T">指定的页面类型</typeparam>
    /// <param name="data">要传递的数据</param>
    /// <param name="isSyncTabRequested">是否同步标签，当在代码中调用时应设为 true</param>
    /// <returns>是否导航成功</returns>
    NavigationResult Navigate<T>(INavigationAwaiter data, bool isSyncTabRequested = false)
        where T : Page;

    /// <summary>
    /// 异步的导航到指定类型的页面
    /// </summary>
    /// <typeparam name="TPage">指定的页面类型</typeparam>
    /// <param name="data">要传递的数据</param>
    /// <param name="syncNavigationViewItem">是否同步标签，当在代码中调用时应设为 true</param>
    /// <returns>是否导航成功</returns>
    Task<NavigationResult> NavigateAsync<TPage>(INavigationAwaiter data, bool syncNavigationViewItem = false)
        where TPage : Page;

    /// <summary>
    /// 同步导航标签
    /// </summary>
    /// <param name="pageType">同步的页面类型</param>
    /// <returns>是否同步成功</returns>
    bool SyncSelectedNavigationViewItemWith(Type pageType);
}
