// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 手动输入方法
/// </summary>
[Injection(InjectAs.Transient, typeof(IGachaLogUrlProvider))]
internal class GachaLogUrlManualInputProvider : IGachaLogUrlProvider
{
    /// <inheritdoc/>
    public string Name { get => nameof(GachaLogUrlManualInputProvider); }

    /// <inheritdoc/>
    public Task<ValueResult<bool, string>> GetQueryAsync()
    {
        throw new NotImplementedException();
    }
}