// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.ViewModel.Wiki;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 角色资料页
/// </summary>
[HighQuality]
internal sealed partial class WikiAvatarPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的角色资料页
    /// </summary>
    public WikiAvatarPage()
    {
        WikiAvatarViewModel viewModel = InitializeWith<WikiAvatarViewModel>();
        InitializeComponent();

        viewModel.Initialize(new AutoSuggestBoxAccessor(AvatarSuggestBox));
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
