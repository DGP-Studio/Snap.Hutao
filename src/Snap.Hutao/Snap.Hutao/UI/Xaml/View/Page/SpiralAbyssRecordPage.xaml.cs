// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.SpiralAbyss;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class SpiralAbyssRecordPage : ScopedPage
{
    public SpiralAbyssRecordPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<SpiralAbyssViewModel>();
    }
}