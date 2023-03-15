// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel.Wiki;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 武器页面
/// </summary>
[HighQuality]
internal sealed partial class WikiWeaponPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的武器页面
    /// </summary>
    public WikiWeaponPage()
    {
        InitializeWith<WikiWeaponViewModel>();
        InitializeComponent();
    }
}
