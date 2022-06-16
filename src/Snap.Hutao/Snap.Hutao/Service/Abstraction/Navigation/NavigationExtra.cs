// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Abstraction.Navigation;

/// <summary>
/// 导航额外信息
/// </summary>
public class NavigationExtra
{
    /// <summary>
    /// 构造一个新的导航额外信息
    /// </summary>
    /// <param name="data">数据</param>
    public NavigationExtra(object? data = null)
    {
        Data = data;
    }

    /// <summary>
    /// 数据
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// 任务完成源
    /// </summary>
    public TaskCompletionSource NavigationCompletedTaskCompletionSource { get; } = new();
}
