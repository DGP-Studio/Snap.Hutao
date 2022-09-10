// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control;

/// <summary>
/// 指示此类支持取消任务
/// </summary>
public interface ISupportCancellation
{
    /// <summary>
    /// 用于通知事件取消的取消令牌
    /// </summary>
    CancellationToken CancellationToken { get; set; }
}