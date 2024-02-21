// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Control.Brush;

[DependencyProperty("Source", typeof(ColorSegmentCollection), default!, nameof(OnSourceChanged))]
internal sealed partial class SegmentedBar : ContentControl
{
    private readonly LinearGradientBrush brush = new() { StartPoint = new(0, 0), EndPoint = new(1, 0), };

    public SegmentedBar()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        Content = new Rectangle()
        {
            Fill = brush,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
    }

    private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        UpdateLinearGradientBrush((SegmentedBar)obj);
    }

    private static void UpdateLinearGradientBrush(SegmentedBar segmentedBar)
    {
        GradientStopCollection collection = segmentedBar.brush.GradientStops;
        collection.Clear();

        ColorSegmentCollection segmentCollection = segmentedBar.Source;

        double total = segmentCollection.Sum(seg => seg.Value);
        if (total is 0D)
        {
            return;
        }

        double offset = 0;
        foreach (ref readonly IColorSegment segment in CollectionsMarshal.AsSpan(segmentCollection))
        {
            collection.Add(new() { Color = segment.Color, Offset = offset, });
            offset += segment.Value / total;
            collection.Add(new() { Color = segment.Color, Offset = offset, });
        }
    }
}