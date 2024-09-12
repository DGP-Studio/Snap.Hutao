// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

internal interface INavigationAwaiter
{
    /// <summary>
    /// 默认的等待器
    /// </summary>
    static readonly INavigationAwaiter Default = new NavigationExtra();

    /// <summary>
    /// 等待导航完成，或直到抛出异常
    /// </summary>
    /// <returns>导航完成的任务</returns>
    Task WaitForCompletionAsync();
}
