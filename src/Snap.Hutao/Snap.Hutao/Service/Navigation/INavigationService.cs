// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航服务
/// </summary>
[HighQuality]
internal interface INavigationService : ICastService, INavigationCurrent
{
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
    /// 若已经处于当前页面不会向页面发送消息
    /// </summary>
    /// <typeparam name="T">指定的页面类型</typeparam>
    /// <param name="data">要传递的数据</param>
    /// <param name="isSyncTabRequested">是否同步标签，当在代码中调用时应设为 true</param>
    /// <returns>是否导航成功</returns>
    NavigationResult Navigate<T>(INavigationAwaiter data, bool isSyncTabRequested = false)
        where T : Page;

    /// <summary>
    /// 异步的导航到指定类型的页面
    /// 若已经处于当前页面则会向页面发送消息
    /// </summary>
    /// <typeparam name="TPage">指定的页面类型</typeparam>
    /// <param name="data">要传递的数据</param>
    /// <param name="syncNavigationViewItem">是否同步标签，当在代码中调用时应设为 true</param>
    /// <returns>是否导航成功</returns>
    ValueTask<NavigationResult> NavigateAsync<TPage>(INavigationAwaiter data, bool syncNavigationViewItem = false)
        where TPage : Page;

    /// <summary>
    /// 尽可能尝试返回
    /// </summary>
    void GoBack();
}

internal interface INavigationCurrent
{
    Type? Current { get; }
}