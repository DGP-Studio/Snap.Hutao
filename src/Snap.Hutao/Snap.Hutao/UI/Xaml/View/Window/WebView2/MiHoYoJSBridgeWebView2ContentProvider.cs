// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
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

        User? user = serviceProvider.GetRequiredService<IUserService>().Current;
        if (user is null || user.SelectedUserGameRole is null)
        {
            return;
        }

        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        if (!UserAndUid.TryFromUser(user, out UserAndUid? userAndUid))
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

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
                .SetCookie(user.CookieToken, user.LToken, userAndUid.IsOversea)
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
        return parentRect;
    }
}