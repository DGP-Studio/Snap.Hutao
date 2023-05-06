// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航服务初始化
/// </summary>
internal interface INavigationInitialization
{
    /// <summary>
    /// 使用指定的对象进行初始化
    /// </summary>
    /// <param name="navigationView">管理的 <see cref="NavigationView"/></param>
    /// <param name="frame">管理的 <see cref="Frame"/></param>
    void Initialize(NavigationView navigationView, Frame frame);
}