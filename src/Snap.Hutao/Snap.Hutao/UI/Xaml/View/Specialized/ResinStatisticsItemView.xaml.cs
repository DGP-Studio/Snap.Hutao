// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.UI.Xaml.Control.Panel;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.Immutable;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[DependencyProperty("Item", typeof(ResinStatisticsItem))]
internal sealed partial class ResinStatisticsItemView : UserControl, IDataRow
{
    private static readonly Size InfiniteSize = new(double.PositiveInfinity, double.PositiveInfinity);

    public ResinStatisticsItemView()
    {
        InitializeComponent();
    }

    public ImmutableArray<double> ColumnsLength
    {
        get
        {
            return RootGrid.Children
                .GroupBy(element => Grid.GetColumn((FrameworkElement)element))
                .Select(group => group.MaxBy(a =>
                {
                    a.Measure(InfiniteSize);
                    return a.DesiredSize.Width;
                }))
                .Select(a => a.DesiredSize.Width)
                .ToImmutableArray();
        }

        set
        {
            ColumnDefinitionCollection collection = RootGrid.ColumnDefinitions;
            for (int index = 0; index < value.Length; index++)
            {
                collection[index].Width = new GridLength(value[index]);
            }
        }
    }
}