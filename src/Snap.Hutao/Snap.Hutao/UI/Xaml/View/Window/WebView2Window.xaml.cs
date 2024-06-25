// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Web.WebView2;

namespace Snap.Hutao.UI.Xaml.View.Window;

internal sealed partial class WebView2Window : Microsoft.UI.Xaml.Window
{
    private readonly CancellationTokenSource loadCts = new();
    private readonly IWebView2ContentProvider contentProvider;

    public WebView2Window(Microsoft.UI.Xaml.Window parentWindow, IWebView2ContentProvider contentProvider)
    {
        this.contentProvider = contentProvider;
        InitializeComponent();

        WebView.Loaded += OnWebViewLoaded;
        WebView.Unloaded += OnWebViewUnloaded;
    }

    private void OnWebViewLoaded(object sender, RoutedEventArgs e)
    {
        OnWebViewLoadedAsync().SafeForget();

        async ValueTask OnWebViewLoadedAsync()
        {
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.DisableDevToolsForReleaseBuild();
            contentProvider.CoreWebView2 = WebView.CoreWebView2;
            await contentProvider.LoadAsync(loadCts.Token).ConfigureAwait(false);
        }
    }

    private void OnWebViewUnloaded(object sender, RoutedEventArgs e)
    {
        loadCts.Cancel();
        loadCts.Dispose();
        contentProvider.Unload();

        WebView.Loaded -= OnWebViewLoaded;
        WebView.Unloaded -= OnWebViewUnloaded;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        contentProvider.ActualTheme = sender.ActualTheme;
    }
}