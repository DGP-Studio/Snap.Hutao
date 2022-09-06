// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 浏览器缓存方法
/// </summary>
[Injection(InjectAs.Transient, typeof(IGachaLogUrlProvider))]
internal class GachaLogUrlWebCacheProvider : IGachaLogUrlProvider
{
    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> GetQueryAsync()
    {
        throw Must.NeverHappen();
    }
}