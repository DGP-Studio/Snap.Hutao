// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Abstraction;

/// <summary>
/// 视图模型接口
/// </summary>
[HighQuality]
internal interface IViewModel
{
    /// <summary>
    /// 用于通知页面卸载的取消令牌
    /// </summary>
    CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 释放操作锁
    /// </summary>
    SemaphoreSlim DisposeLock { get; set; }

    /// <summary>
    /// 对应的视图是否已经释放
    /// </summary>
    bool IsViewDisposed { get; set; }
}