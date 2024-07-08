// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Graphics;
using System.Diagnostics;
using Windows.Graphics;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

internal sealed class UpdateLogContentProvider : IWebView2ContentProvider
{
    public ElementTheme ActualTheme { get; set; }

    public CoreWebView2? CoreWebView2 { get; set; }

    public ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(CoreWebView2);
        CoreWebView2.NewWindowRequested += OnNewWindowRequested;
        CoreWebView2.Navigate("https://hut.ao/statements/latest.html");
        return ValueTask.CompletedTask;
    }

    public RectInt32 InitializePosition(RectInt32 parentRect, double parentDpi)
    {
        PointInt32 center = parentRect.GetPointInt32(PointInt32Kind.Center);
        SizeInt32 size = new SizeInt32(640, 800).Scale(parentDpi);
        RectInt32 target = RectInt32Convert.RectInt32(new(center.X - (size.Width / 2), center.Y - (size.Height / 2)), size);
        RectInt32 workArea = DisplayArea.GetFromRect(parentRect, DisplayAreaFallback.None).WorkArea;
        RectInt32 workAreaShrink = new(workArea.X + 48, workArea.Y + 48, workArea.Width - 96, workArea.Height - 96);

        if (target.Width > workAreaShrink.Width)
        {
            target.Width = workAreaShrink.Width;
        }

        if (target.Height > workAreaShrink.Height)
        {
            target.Height = workAreaShrink.Height;
        }

        PointInt32 topLeft = target.GetPointInt32(PointInt32Kind.TopLeft);

        if (topLeft.X < workAreaShrink.X)
        {
            target.X = workAreaShrink.X;
        }

        if (topLeft.Y < workAreaShrink.Y)
        {
            target.Y = workAreaShrink.Y;
        }

        PointInt32 bottomRight = target.GetPointInt32(PointInt32Kind.BottomRight);
        PointInt32 workAreeShrinkBottomRight = workAreaShrink.GetPointInt32(PointInt32Kind.BottomRight);

        if (bottomRight.X > workAreeShrinkBottomRight.X)
        {
            target.X = workAreeShrinkBottomRight.X - target.Width;
        }

        if (bottomRight.Y > workAreeShrinkBottomRight.Y)
        {
            target.Y = workAreeShrinkBottomRight.Y - target.Height;
        }

        return target;
    }

    public void Unload()
    {
        if (CoreWebView2 is not null)
        {
            CoreWebView2.NewWindowRequested -= OnNewWindowRequested;
        }
    }

    private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled = true;
        _ = Launcher.LaunchUriAsync(e.Uri.ToUri());
    }
}