// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 指示该类为用户服务初始化器
/// </summary>
internal interface IHutaoUserServiceInitialization
{
    /// <summary>
    /// 异步初始化
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>任务</returns>
    ValueTask InitializeInternalAsync(CancellationToken token = default);
}