// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service;
using Windows.Graphics;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

internal sealed class UpdateLogContentProvider : IWebView2ContentProvider
{
    private string? languageCode;

    public ElementTheme ActualTheme { get; set; }

    public CoreWebView2? CoreWebView2 { get; set; }

    public Action? CloseWindowAction { get; set; }

    public ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        languageCode = serviceProvider.GetRequiredService<CultureOptions>().LanguageCode;

        ArgumentNullException.ThrowIfNull(CoreWebView2);
        CoreWebView2.AddWebResourceRequestedFilter("https://hut.ao/statements/latest.html", CoreWebView2WebResourceContext.Document);
        CoreWebView2.NewWindowRequested += OnNewWindowRequested;
        CoreWebView2.WebResourceRequested += OnWebResourceRequested;
        CoreWebView2.Navigate("https://hut.ao/statements/latest.html");
        return ValueTask.CompletedTask;
    }

    public RectInt32 InitializePosition(RectInt32 parentRect, double parentDpi)
    {
        return WebView2WindowPosition.Vertical(parentRect, parentDpi);
    }

    public void Unload()
    {
        if (CoreWebView2 is not null)
        {
            CoreWebView2.NewWindowRequested -= OnNewWindowRequested;
            CoreWebView2.WebResourceRequested -= OnWebResourceRequested;
        }
    }

    private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled = true;
        _ = Launcher.LaunchUriAsync(e.Uri.ToUri());
    }

    private void OnWebResourceRequested(CoreWebView2 coreWebView2, CoreWebView2WebResourceRequestedEventArgs args)
    {
        args.Request.Headers.SetHeader("Accept-Language", languageCode);
    }
}