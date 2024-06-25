// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.SpiralAbyss;

namespace Snap.Hutao.UI.Xaml.View.Page;

/// <summary>
/// 深渊记录页面
/// </summary>
[HighQuality]
internal sealed partial class SpiralAbyssRecordPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的深渊记录页面
    /// </summary>
    public SpiralAbyssRecordPage()
    {
        InitializeWith<SpiralAbyssRecordViewModel>();
        InitializeComponent();
    }
}
