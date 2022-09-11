// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 祈愿记录视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class GachaLogViewModel : ISupportCancellation
{
    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }
}
