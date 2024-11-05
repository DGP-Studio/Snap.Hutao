// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[DependencyProperty("Title", typeof(string))]
[DependencyProperty("Countdowns", typeof(List<Countdown>))]
internal sealed partial class CountdownCard : UserControl
{
    public CountdownCard()
    {
        InitializeComponent();
    }
}