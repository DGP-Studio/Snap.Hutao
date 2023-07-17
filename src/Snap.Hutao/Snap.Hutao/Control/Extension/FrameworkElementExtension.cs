// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Extension;

internal static class FrameworkElementExtension
{
    /// <summary>
    /// Make properties below false:
    /// <code>
    /// * AllowFocusOnInteraction
    /// * IsDoubleTapEnabled
    /// * IsHitTestVisible
    /// * IsHoldingEnabled
    /// * IsRightTapEnabled
    /// * IsTabStop
    /// </code>
    /// </summary>
    /// <param name="frameworkElement">元素</param>
    public static void DisableInteraction(this FrameworkElement frameworkElement)
    {
        frameworkElement.AllowFocusOnInteraction = false;
        frameworkElement.IsDoubleTapEnabled = false;
        frameworkElement.IsHitTestVisible = false;
        frameworkElement.IsHoldingEnabled = false;
        frameworkElement.IsRightTapEnabled = false;
        frameworkElement.IsTabStop = false;
    }
}