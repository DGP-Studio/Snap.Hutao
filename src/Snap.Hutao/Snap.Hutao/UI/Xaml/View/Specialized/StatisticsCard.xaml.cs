// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[DependencyProperty("ShowUpPull", typeof(bool), true)]
internal sealed partial class StatisticsCard : UserControl
{
    public StatisticsCard()
    {
        InitializeComponent();
    }
}
