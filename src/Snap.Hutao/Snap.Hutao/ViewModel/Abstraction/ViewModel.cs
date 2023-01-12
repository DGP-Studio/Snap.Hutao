// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.ViewModel.Abstraction;

/// <summary>
/// 视图模型抽象类
/// </summary>
public abstract class ViewModel : ObservableObject, IViewModel
{
    private bool isInitialized;

    /// <summary>
    /// 是否初始化完成
    /// </summary>
    public bool IsInitialized { get => isInitialized; set => SetProperty(ref isInitialized, value); }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <inheritdoc/>
    public SemaphoreSlim DisposeLock { get; set; } = new(1);

    /// <inheritdoc/>
    public bool IsViewDisposed { get; set; }

    /// <summary>
    /// 当页面被释放后抛出异常
    /// </summary>
    /// <exception cref="OperationCanceledException">操作被用户取消</exception>
    protected void ThrowIfViewDisposed()
    {
        if (IsViewDisposed)
        {
            throw new OperationCanceledException("页面资源已经被释放，操作取消");
        }
    }
}