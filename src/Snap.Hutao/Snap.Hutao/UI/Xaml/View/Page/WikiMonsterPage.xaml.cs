// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.Wiki;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class WikiMonsterPage : ScopedPage
{
    public WikiMonsterPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<WikiMonsterViewModel>();
    }
}