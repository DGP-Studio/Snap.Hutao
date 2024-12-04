// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

internal sealed class GeetestWebView2ContentProvider : DependencyObject, IWebView2ContentProvider
{
    private readonly string gt;
    private readonly string challenge;
    private readonly bool isOversea;
    private readonly TaskCompletionSource resultTcs = new();

    private string? result;

    public GeetestWebView2ContentProvider(string gt, string challenge, bool isOversea)
    {
        this.gt = gt;
        this.challenge = challenge;
        this.isOversea = isOversea;
    }

    public ElementTheme ActualTheme { get; set; }

    public CoreWebView2? CoreWebView2 { get; set; }

    public Action? CloseWindowAction { get; set; }

    public ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(CoreWebView2);

        CoreWebView2.WebMessageReceived += OnWebMessageReceived;

        CoreWebView2.NavigateToString($$"""
            <html>
                <head>
                    <title>{{SH.UIXamlViewWindowWebView2GeetestHeader}}</title>
                    <style>
                        #geetest-div {
                            aligin-items:center
                        }
                    </style>
                </head>
                <body>
                    <div id="geetest-div"></div>
                </body>
                <script src="https://static.geetest.com/static/js/gt.0.5.2.js"></script>
                <script>
                    initGeetest(
                        {
                            protocol: "https://",
                            gt: "{{gt}}",
                            challenge: "{{challenge}}",
                            new_captcha: true,
                            product: 'bind',
                            api_server: '{{(isOversea ? "api-na.geetest.com" : "api.geetest.com")}}'
                        },
                        function (captchaObj) {
                            captchaObj.onReady(function () {
                                captchaObj.verify();
                            });
                            captchaObj.onSuccess(function () {
                                var result = captchaObj.getValidate();
                                chrome.webview.postMessage(result);
                            });
                        }
                    );
              </script>
            </html>
            """);

        return ValueTask.CompletedTask;
    }

    public void Unload()
    {
        resultTcs.TrySetResult();
    }

    public RectInt32 InitializePosition(RectInt32 parentRect, double parentDpi)
    {
        return WebView2WindowPosition.Vertical(parentRect, parentDpi);
    }

    public async ValueTask<string?> GetResultAsync()
    {
        await resultTcs.Task.ConfigureAwait(false);
        return result;
    }

    private void OnWebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        result = args.WebMessageAsJson;
        CloseWindowAction?.Invoke();
    }
}