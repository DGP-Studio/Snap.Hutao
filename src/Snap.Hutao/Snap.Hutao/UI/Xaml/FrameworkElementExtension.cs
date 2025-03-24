// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.UI.Xaml;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? DataContext<T>(this FrameworkElement element)
        where T : class
    {
        return element.DataContext as T;
    }

    public static void InitializeDataContext<TDataContext>(this FrameworkElement frameworkElement, IServiceProvider serviceProvider)
        where TDataContext : class
    {
        try
        {
            frameworkElement.DataContext = serviceProvider.GetRequiredService<TDataContext>();
            (frameworkElement as IDataContextInitialized)?.OnDataContextInitialized(serviceProvider);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }
}