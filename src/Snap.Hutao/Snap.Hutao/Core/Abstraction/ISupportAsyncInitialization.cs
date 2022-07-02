// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Abstraction;

/// <summary>
/// 可异步初始化
/// </summary>
internal interface ISupportAsyncInitialization
{
    /// <summary>
    /// 是否已经初始化完成
    /// </summary>
    public bool IsInitialized { get; }

    /// <summary>
    /// 异步初始化
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>初始化任务</returns>
    ValueTask<bool> InitializeAsync(CancellationToken token = default);
}