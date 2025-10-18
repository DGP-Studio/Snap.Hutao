// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel.Abstraction;

namespace Snap.Hutao.UI.Xaml.View.Card;

internal sealed partial class GachaStatisticsCard : Button
{
    public GachaStatisticsCard(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        this.InitializeViewModelSlim<ViewModel.GachaLog.GachaLogViewModelSlim>(serviceProvider);
    }
}