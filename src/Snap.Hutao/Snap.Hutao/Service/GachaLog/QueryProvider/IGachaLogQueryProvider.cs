// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 祈愿记录Url提供器
/// </summary>
[HighQuality]
internal interface IGachaLogQueryProvider : INamedService
{
    /// <summary>
    /// 异步获取包含验证密钥的查询语句
    /// 查询语句可以仅包含?后的内容
    /// </summary>
    /// <returns>包含验证密钥的查询语句</returns>
    Task<ValueResult<bool, GachaLogQuery>> GetQueryAsync();
}