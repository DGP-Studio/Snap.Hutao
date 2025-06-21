// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.Achievement;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class AchievementPage : ScopedPage
{
    public AchievementPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<AchievementViewModel>();
    }
}