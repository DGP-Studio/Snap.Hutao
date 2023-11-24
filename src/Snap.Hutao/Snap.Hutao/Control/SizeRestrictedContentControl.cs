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

            if (IsWidthRestricted)
            {
                if (contentDesiredSize.Width > minContentWidth)
                {
                    minContentWidth = contentDesiredSize.Width;
                }

                element.MinWidth = minContentWidth;
            }

            if (IsHeightRestricted)
            {
                if (contentDesiredSize.Height > minContentHeight)
                {
                    minContentHeight = contentDesiredSize.Height;
                }

                element.MinHeight = minContentHeight;
            }
        }

        return base.MeasureOverride(availableSize);
    }
}