// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Control.Panel;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.View.Control;

[DependencyProperty("Statistics", typeof(HutaoStatistics), default, nameof(OnStatisticsChanged))]
internal sealed partial class HutaoStatisticsPanel : HorizontalEqualPanel
{
    public HutaoStatisticsPanel()
    {
        InitializeComponent();

        if (DataContext is HutaoStatistics statistics && statistics.Chronicled is { } chronicled)
        {
            Children.Add(new HutaoStatisticsCard
            {
                DataContext = chronicled,
            });
        }
    }

    private static void OnStatisticsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        HutaoStatisticsPanel panel = (HutaoStatisticsPanel)obj;

        if (args.NewValue is HutaoStatistics { Chronicled: { } chronicled })
        {
            panel.Children.Add(new HutaoStatisticsCard
            {
                DataContext = chronicled,
            });
        }
    }
}
