// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航服务初始化
/// </summary>
internal interface INavigationInitialization
{
    void Initialize(INavigationViewAccessor accessor);
}