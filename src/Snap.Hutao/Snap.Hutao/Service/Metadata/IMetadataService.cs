// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据服务
/// </summary>
[HighQuality]
internal interface IMetadataService : ICastService,
    IMetadataServiceRawData,
    IMetadataServiceIdDataMap,
    IMetadataServiceNameDataMap,
    IMetadataServiceNameLevelCurveMap
{
    /// <summary>
    /// 异步初始化服务，尝试更新元数据
    /// </summary>
    /// <returns>初始化是否成功</returns>
    ValueTask<bool> InitializeAsync();
}