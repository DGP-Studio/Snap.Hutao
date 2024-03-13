// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据服务
/// </summary>
[HighQuality]
internal interface IMetadataService
{
    IMemoryCache MemoryCache { get; }

    ValueTask<T> FromCacheOrFileAsync<T>(string fileName, CancellationToken token)
        where T : class;

    /// <summary>
    /// 异步初始化服务，尝试更新元数据
    /// </summary>
    /// <returns>初始化是否成功</returns>
    ValueTask<bool> InitializeAsync();
}