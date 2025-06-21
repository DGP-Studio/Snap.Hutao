// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.HutaoPassport;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class HutaoPassportPage : ScopedPage
{
    public HutaoPassportPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<HutaoPassportViewModel>();
    }
}