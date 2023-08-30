// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Control;

[DependencyProperty("Source", typeof(List<ColorSegment>), default!, nameof(OnSourceChanged))]
internal sealed partial class SegmentedBar : ContentControl
{
    private readonly LinearGradientBrush brush = new();

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

        segmentedBar.brush.GradientStops.Clear();

        if (args.NewValue as List<ColorSegment> is [_, ..] list)
        {
            foreach (ref readonly ColorSegment segment in CollectionsMarshal.AsSpan(list))
            {

            }
        }
    }
}