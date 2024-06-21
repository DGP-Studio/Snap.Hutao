// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 祈愿记录页面
/// </summary>
[HighQuality]
internal sealed partial class GachaLogPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的祈愿记录页面
    /// </summary>
    public GachaLogPage()
    {
        InitializeWith<GachaLogViewModel>();
        InitializeComponent();
    }
}
