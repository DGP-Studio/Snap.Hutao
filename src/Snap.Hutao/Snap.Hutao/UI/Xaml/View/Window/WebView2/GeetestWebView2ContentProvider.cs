// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Graphics;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

internal sealed partial class GeetestWebView2ContentProvider : DependencyObject, IWebView2ContentProvider
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
        PointInt32 center = parentRect.GetPointInt32(PointInt32Kind.Center);
        SizeInt32 size = new SizeInt32(480, 800).Scale(parentDpi);
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