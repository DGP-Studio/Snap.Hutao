// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航结果
/// </summary>
public enum NavigationResult
{
    /// <summary>
    /// 成功
    /// </summary>
    Succeed,

    /// <summary>
    /// 失败
    /// </summary>
    Failed,

    /// <summary>
    /// 已经处于该页面
    /// </summary>
    AlreadyNavigatedTo,
}