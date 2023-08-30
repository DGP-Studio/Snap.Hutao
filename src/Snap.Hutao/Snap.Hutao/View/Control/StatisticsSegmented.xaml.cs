// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Control;

[DependencyProperty("IsPredictPullAvailable", typeof(bool), false, nameof(OnIsPredictPullAvailableChanged))]
internal sealed partial class StatisticsSegmented : Segmented
{
    private readonly SegmentedItem predictPullItem = new()
    {
        Content = SH.ViewControlStatisticsSegmentedItemContentPrediction,
        Icon = new FontIcon() { Glyph = "\uEA80" },
    };

    public StatisticsSegmented()
    {
        InitializeComponent();
    }

    private static void OnIsPredictPullAvailableChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        StatisticsSegmented segmented = (StatisticsSegmented)obj;
        if (args.NewValue is true)
        {
            segmented.Items.Add(segmented.predictPullItem);
        }
        else
        {
            segmented.Items.RemoveLast();
        }
    }
}
