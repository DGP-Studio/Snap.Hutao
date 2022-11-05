// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Control;
using Snap.Hutao.Model;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实时便笺视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class DailyNoteViewModel : ObservableObject, ISupportCancellation
{
    private readonly List<NamedValue<int>> refreshTimes = new()
    {
        new("4 分钟 | 0.5 树脂", 240),
        new("8 分钟 | 1 树脂", 480),
        new("30 分钟 | 3.75 树脂", 1800),
        new("40 分钟 | 5 树脂", 2400),
        new("60 分钟 | 7.5 树脂", 3600),
    };

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 刷新时间
    /// </summary>
    public List<NamedValue<int>> RefreshTimes { get => refreshTimes; }
}
