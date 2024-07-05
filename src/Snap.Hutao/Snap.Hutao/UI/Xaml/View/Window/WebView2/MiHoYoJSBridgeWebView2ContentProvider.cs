// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Bridge;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

[DependencyProperty("SourceProvider", typeof(IJSBridgeUriSourceProvider))]
internal sealed partial class MiHoYoJSBridgeWebView2ContentProvider : DependencyObject, IWebView2ContentProvider
{
    private MiHoYoJSBridgeFacade? jsBridge;

    public ElementTheme ActualTheme { get; set; }

    public CoreWebView2? CoreWebView2 { get; set; }

    public async ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(CoreWebView2);

        if (SourceProvider is null)
        {
            return;
        }

        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        if (await serviceProvider.GetRequiredService<IUserService>().GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        await serviceProvider.GetRequiredService<ITaskContext>().SwitchToMainThreadAsync();
        string source = SourceProvider.GetSource(userAndUid);
        if (!string.IsNullOrEmpty(source))
        {
            CoreWebView2Navigator navigator = new(CoreWebView2);
            await navigator.NavigateAsync("about:blank").ConfigureAwait(true);

            try
            {
                await CoreWebView2.Profile.ClearBrowsingDataAsync();
            }
            catch (InvalidCastException)
            {
                infoBarService.Warning(SH.ViewControlWebViewerCoreWebView2ProfileQueryInterfaceFailed);
                await CoreWebView2.DeleteCookiesAsync(userAndUid.IsOversea).ConfigureAwait(true);
            }

            CoreWebView2
                .SetCookie(userAndUid.User.CookieToken, userAndUid.User.LToken, userAndUid.IsOversea)
                .SetMobileUserAgent(userAndUid.IsOversea);
            jsBridge = SourceProvider.CreateJSBridge(serviceProvider, CoreWebView2, userAndUid);

            await navigator.NavigateAsync(source).ConfigureAwait(true);
            await CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.BrowsingHistory);
        }
    }

    public void Unload()
    {
        jsBridge?.Detach();
    }

    public RectInt32 InitializePosition(RectInt32 parentRect)
    {
        PointInt32 center = parentRect.GetPointInt32(PointInt32Kind.Center);
        RectInt32 target = new(center.X - 240, center.Y - 400, 450, 800);

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
}