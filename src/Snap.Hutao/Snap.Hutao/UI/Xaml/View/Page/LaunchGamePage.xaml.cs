// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.Game;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class LaunchGamePage : ScopedPage
{
    public LaunchGamePage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<LaunchGameViewModel>();
    }
}