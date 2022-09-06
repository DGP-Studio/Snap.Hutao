// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录Url提供器
/// </summary>
public interface IGachaLogUrlProvider
{
    /// <summary>
    /// 异步获取包含验证密钥的查询语句
    /// </summary>
    /// <returns>包含验证密钥的查询语句</returns>
    Task<ValueResult<bool, string>> GetQueryAsync();
}
