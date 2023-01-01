// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 深渊记录页面
/// </summary>
public sealed partial class SpiralAbyssRecordPage : ScopedPage
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
