// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class AvatarPropertyPage : ScopedPage
{
    public AvatarPropertyPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<AvatarPropertyViewModel>();
    }
}