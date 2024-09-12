﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.Cultivation;

namespace Snap.Hutao.UI.Xaml.View.Page;

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
