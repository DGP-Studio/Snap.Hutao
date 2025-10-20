// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.UI.Windowing.Abstraction;
using Windows.Foundation;
using Windows.Graphics;

namespace Snap.Hutao.UI.Windowing;

internal static class XamlWindowRegionRects
{
    public static void Update(Window window)
    {
        if (window is not IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            return;
        }

        // E_UNEXPECTED will be thrown if the Content is not loaded.
        if (!xamlWindow.TitleBarCaptionAccess.IsLoaded)
        {
            return;
        }

        InputNonClientPointerSource inputNonClientPointerSource = InputNonClientPointerSource.GetForWindowId(window.AppWindow.Id);
        {
            FrameworkElement element = xamlWindow.TitleBarCaptionAccess;
            Point position = element.TransformToVisual(window.Content).TransformPoint(default);
            RectInt32 rect = RectInt32Convert.RectInt32(position, element.ActualSize).Scale(window.GetRasterizationScale());
            inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Caption, [rect]);
        }

        List<RectInt32> passthrough = [];
        foreach (FrameworkElement element in xamlWindow.TitleBarPassthrough)
        {
            if (element.Visibility is not Visibility.Visible)
            {
                continue;
            }

            Point position = element.TransformToVisual(window.Content).TransformPoint(default);
            RectInt32 rect = RectInt32Convert.RectInt32(position, element.ActualSize).Scale(window.GetRasterizationScale());

            if (rect.Size() > 0)
            {
                passthrough.Add(rect);
            }
        }

        if (passthrough.Count > 0)
        {
            inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Passthrough, [.. passthrough]);
        }
    }
}