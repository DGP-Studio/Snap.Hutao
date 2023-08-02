// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 指示该类为元数据初始化器
/// </summary>
[HighQuality]
internal interface IMetadataServiceInitialization
{
    /// <summary>
    /// 异步初始化元数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>任务</returns>
    ValueTask InitializeInternalAsync(CancellationToken token = default);
}