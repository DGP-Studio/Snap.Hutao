// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 胡桃通行证页面
/// </summary>
internal sealed partial class HutaoPassportPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的胡桃通行证页面
    /// </summary>
    public HutaoPassportPage()
    {
        InitializeWith<HutaoPassportViewModel>();
        InitializeComponent();
    }
}
