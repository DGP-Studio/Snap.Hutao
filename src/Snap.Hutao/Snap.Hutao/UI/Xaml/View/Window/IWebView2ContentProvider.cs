// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;

namespace Snap.Hutao.UI.Xaml.View.Window;

internal interface IWebView2ContentProvider
{
    ElementTheme ActualTheme { get; set; }

    CoreWebView2? CoreWebView2 { get; set; }

    ValueTask LoadAsync(CancellationToken token);

    void Unload();
}