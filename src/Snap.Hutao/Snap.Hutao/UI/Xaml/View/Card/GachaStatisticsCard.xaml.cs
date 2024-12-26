// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Card;

internal sealed partial class GachaStatisticsCard : Button
{
    public GachaStatisticsCard()
    {
        this.InitializeDataContext<ViewModel.GachaLog.GachaLogViewModelSlim>();
        InitializeComponent();
    }
}