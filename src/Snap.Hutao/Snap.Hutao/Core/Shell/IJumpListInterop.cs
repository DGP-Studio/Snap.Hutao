// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Shell;

/// <summary>
/// 跳转列表交互
/// </summary>
internal interface IJumpListInterop
{
    /// <summary>
    /// 异步配置跳转列表
    /// </summary>
    /// <returns>任务</returns>
    ValueTask ConfigureAsync();
}