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

    public static void InitializeDataContext<TDataContext>(this FrameworkElement frameworkElement, IServiceProvider? serviceProvider = default)
        where TDataContext : class
    {
        IServiceProvider service = serviceProvider ?? Ioc.Default;
        try
        {
            frameworkElement.DataContext = service.GetRequiredService<TDataContext>();
        }
        catch (Exception ex)
        {
            ILogger? logger = service.GetRequiredService(typeof(ILogger<>).MakeGenericType([frameworkElement.GetType()])) as ILogger;
            logger?.LogError(ex, "Failed to initialize DataContext");
            throw;
        }
    }
}