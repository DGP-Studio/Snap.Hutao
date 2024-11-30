// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel.Cultivation;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[DependencyProperty("Item", typeof(ResinStatisticsItem))]
internal sealed partial class ResinStatisticsItemView : UserControl
{
    public ResinStatisticsItemView()
    {
        InitializeComponent();
    }
}