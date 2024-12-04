// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

internal interface IWebView2ContentProvider
{
    ElementTheme ActualTheme { get; set; }

    CoreWebView2? CoreWebView2 { get; set; }

    Action? CloseWindowAction { get; set; }

    ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token);

    RectInt32 InitializePosition(RectInt32 parentRect, double parentDpi);

    void Unload();
}