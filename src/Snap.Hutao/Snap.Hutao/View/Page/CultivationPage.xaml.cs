// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 养成页面
/// </summary>
[HighQuality]
internal sealed partial class CultivationPage : ScopedPage
{
    /// <summary>
    /// 够造一个新的养成页面
    /// </summary>
    public CultivationPage()
    {
        InitializeWith<CultivationViewModel>();
        InitializeComponent();
    }
}
