// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Windows.Foundation;

namespace Snap.Hutao.Web.Bridge;

internal sealed class CoreWebView2Navigator
{
    private readonly TypedEventHandler<CoreWebView2, CoreWebView2NavigationCompletedEventArgs> coreWebView2NavigationCompletedEventHandler;
    private readonly CoreWebView2 coreWebView2;
    private TaskCompletionSource navigationTask = new();

    public CoreWebView2Navigator(CoreWebView2 coreWebView2)
    {
        coreWebView2NavigationCompletedEventHandler = OnWebviewNavigationCompleted;
        this.coreWebView2 = coreWebView2;
    }

    public async ValueTask NavigateAsync(string url)
    {
        coreWebView2.NavigationCompleted += coreWebView2NavigationCompletedEventHandler;
        coreWebView2.Navigate(url);
        await navigationTask.Task.ConfigureAwait(false);
        coreWebView2.NavigationCompleted -= coreWebView2NavigationCompletedEventHandler;
    }

    private void OnWebviewNavigationCompleted(CoreWebView2 webView2, CoreWebView2NavigationCompletedEventArgs args)
    {
        navigationTask.TrySetResult();
        navigationTask = new();
    }
}