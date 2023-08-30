// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace Snap.Hutao.Control;

[DependencyProperty("Source", typeof(GradientStopCollection))]
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
}