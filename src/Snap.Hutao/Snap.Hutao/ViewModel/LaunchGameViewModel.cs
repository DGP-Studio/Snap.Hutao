// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Control;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 启动游戏视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class LaunchGameViewModel : ObservableObject, ISupportCancellation
{
    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }
}
