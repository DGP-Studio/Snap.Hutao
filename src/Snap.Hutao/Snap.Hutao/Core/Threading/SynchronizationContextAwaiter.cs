// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 同步上下文等待器
/// </summary>
public struct SynchronizationContextAwaiter : INotifyCompletion
{
    private static readonly SendOrPostCallback PostCallback = state => ((Action)state!)();
    private readonly SynchronizationContext context;

    /// <summary>
    /// 构造一个新的同步上下文等待器
    /// </summary>
    /// <param name="context">同步上下文</param>
    public SynchronizationContextAwaiter(SynchronizationContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// 是否完成
    /// </summary>
    public bool IsCompleted => context == SynchronizationContext.Current;

    /// <summary>
    /// 完成操作
    /// </summary>
    /// <param name="continuation">后续操作</param>
    [SuppressMessage("", "VSTHRD001")]
    public void OnCompleted(Action continuation) => context.Post(PostCallback, continuation);

    /// <summary>
    /// 获取执行结果
    /// </summary>
    public void GetResult()
    {
    }
}