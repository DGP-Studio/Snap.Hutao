// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Control;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 养成视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class CultivationViewModel : ObservableObject, ISupportCancellation
{
    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }
}