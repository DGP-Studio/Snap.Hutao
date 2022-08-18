// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 同步上下文等待体
/// </summary>
public struct SynchronizationContextAwaitable
{
    private readonly SynchronizationContext context;

    /// <summary>
    /// 构造一个新的同步上下文等待体
    /// </summary>
    /// <param name="context">同步上下文</param>
    public SynchronizationContextAwaitable(SynchronizationContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// 获取等待器
    /// </summary>
    /// <returns>等待器</returns>
    public SynchronizationContextAwaiter GetAwaiter()
    {
        return new SynchronizationContextAwaiter(context);
    }
}
