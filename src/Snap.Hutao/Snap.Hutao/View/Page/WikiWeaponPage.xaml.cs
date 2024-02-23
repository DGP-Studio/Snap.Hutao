// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
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
        WikiWeaponViewModel viewModel = InitializeWith<WikiWeaponViewModel>();
        InitializeComponent();

        viewModel.Initialize(new AutoSuggestBoxAccessor(WeaponSuggestBox));
    }

    private class AutoSuggestBoxAccessor : IAutoSuggestBoxAccessor
    {
        public AutoSuggestBoxAccessor(AutoSuggestBox autoSuggestBox)
        {
            AutoSuggestBox = autoSuggestBox;
        }

        public AutoSuggestBox AutoSuggestBox { get; private set; }
    }
}
