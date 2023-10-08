// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Control.Brush;

[DependencyProperty("Source", typeof(List<IColorSegment>), default!, nameof(OnSourceChanged))]
internal sealed partial class SegmentedBar : ContentControl
{
    private readonly LinearGradientBrush brush = new() { StartPoint = new(0, 0), EndPoint = new(1, 0), };

    public SegmentedBar()
    {
        Content = new Rectangle()
        {
            Fill = brush,
        };
    }

    private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        SegmentedBar segmentedBar = (SegmentedBar)obj;

        GradientStopCollection collection = segmentedBar.brush.GradientStops;
        collection.Clear();

        if (args.NewValue as List<IColorSegment> is [_, ..] list)
        {
            double total = list.Sum(seg => seg.Value);
            double offset = 0;
            foreach (ref readonly IColorSegment segment in CollectionsMarshal.AsSpan(list))
            {
                collection.Add(new GradientStop() { Color = segment.Color, Offset = offset, });
                offset += segment.Value / total;
                collection.Add(new GradientStop() { Color = segment.Color, Offset = offset, });
            }
        }
    }
}