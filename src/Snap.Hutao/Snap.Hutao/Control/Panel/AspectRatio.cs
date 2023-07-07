// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Foundation;

namespace Snap.Hutao.Control.Panel;

/// <summary>
/// 纵横比控件
/// </summary>
[HighQuality]
[DependencyProperty("TargetWidth", typeof(double), 1.0D)]
[DependencyProperty("TargetHeight", typeof(double), 1.0D)]
internal sealed partial class AspectRatio : Microsoft.UI.Xaml.Controls.Control
{
    private const double Epsilon = 2.2204460492503131e-016;

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        double ratio = TargetWidth / TargetHeight;
        double ratioAvailable = availableSize.Width / availableSize.Height;

        if (Math.Abs(ratioAvailable - ratio) < Epsilon)
        {
            return availableSize;
        }

        // 更宽
        if (ratioAvailable > ratio)
        {
            double newWidth = ratio * availableSize.Height;
            return new Size(newWidth, availableSize.Height);
        }

        // 更高
        if (ratioAvailable < ratio)
        {
            double newHeight = availableSize.Width / ratio;
            return new Size(availableSize.Width, newHeight);
        }

        return availableSize;
    }
}
