// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Snap.Hutao.Control;

[DependencyProperty("IsWidthRestricted", typeof(bool), true)]
[DependencyProperty("IsHeightRestricted", typeof(bool), true)]
internal sealed partial class SizeRestrictedContentControl : ContentControl
{
    private double minContentWidth;
    private double minContentHeight;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Content is FrameworkElement element)
        {
            element.Measure(availableSize);
            Size contentDesiredSize = element.DesiredSize;
            Size contentActualOrDesiredSize = new(
                Math.Max(element.ActualWidth, contentDesiredSize.Width),
                Math.Max(element.ActualHeight, contentDesiredSize.Height));

            if (IsWidthRestricted)
            {
                if (contentActualOrDesiredSize.Width > minContentWidth)
                {
                    minContentWidth = contentActualOrDesiredSize.Width;
                }

                element.MinWidth = minContentWidth;
            }

            if (IsHeightRestricted)
            {
                if (contentActualOrDesiredSize.Height > minContentHeight)
                {
                    minContentHeight = contentActualOrDesiredSize.Height;
                }

                element.MinHeight = minContentHeight;
            }
        }

        return base.MeasureOverride(availableSize);
    }
}