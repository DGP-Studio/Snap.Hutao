// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel.Complex;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 胡桃数据库页面
/// </summary>
[HighQuality]
internal sealed partial class HutaoDatabasePage : ScopedPage
{
    /// <summary>
    /// 构造一个新的胡桃数据库页面
    /// </summary>
    public HutaoDatabasePage()
    {
        InitializeWith<HutaoDatabaseViewModel>();
        InitializeComponent();
    }
}
