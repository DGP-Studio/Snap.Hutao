// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 提供简单易用的状态提示信息
/// 用于任务的状态跟踪
/// 同时继承了 <see cref="Observable"/>
/// </summary>
public class Watcher : Observable
{
    private readonly bool isReusable;
    private bool hasUsed;
    private bool isWorking;
    private bool isCompleted;

    /// <summary>
    /// 构造一个新的工作监视器
    /// </summary>
    /// <param name="isReusable">是否可以重用</param>
    public Watcher(bool isReusable = true)
    {
        this.isReusable = isReusable;
    }

    /// <summary>
    /// 是否正在工作
    /// </summary>
    public bool IsWorking
    {
        get => isWorking;

        private set => Set(ref isWorking, value);
    }

    /// <summary>
    /// 工作是否完成
    /// </summary>
    public bool IsCompleted
    {
        get => isCompleted;

        private set => Set(ref isCompleted, value);
    }

    /// <summary>
    /// 对某个操作进行监视，
    /// 无法防止代码重入
    /// </summary>
    /// <returns>一个可释放的对象，用于在操作完成时自动提示监视器工作已经完成</returns>
    /// <exception cref="InvalidOperationException">重用了一个不可重用的监视器</exception>
    public IDisposable Watch()
    {
        Verify.Operation(isReusable || !hasUsed, $"此 {nameof(Watcher)} 不允许多次使用");

        hasUsed = true;
        IsWorking = true;

        return new WorkDisposable(this);
    }

    private struct WorkDisposable : IDisposable
    {
        private readonly Watcher work;

        public WorkDisposable(Watcher work)
        {
            this.work = work;
        }

        public void Dispose()
        {
            work.IsWorking = false;
            work.IsCompleted = true;
        }
    }
}
