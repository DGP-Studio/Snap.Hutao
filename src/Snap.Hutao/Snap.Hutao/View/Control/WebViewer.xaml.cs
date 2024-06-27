// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Message;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.WebView2;
using Windows.Foundation;

namespace Snap.Hutao.View.Control;

[DependencyProperty("SourceProvider", typeof(IJSBridgeUriSource))]
[DependencyProperty("DocumentTitle", typeof(string))]
[DependencyProperty("CanGoBack", typeof(bool))]
internal partial class WebViewer : UserControl, IRecipient<UserChangedMessage>
{
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly RoutedEventHandler loadEventHandler;
    private readonly TypedEventHandler<CoreWebView2, object> documentTitleChangedEventHandler;
    private readonly TypedEventHandler<CoreWebView2, object> historyChangedEventHandler;

    private MiHoYoJSBridgeFacade? jsBridge;
    private bool isInitializingOrInitialized;

    public WebViewer()
    {
        Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00000000");
        InitializeComponent();
        serviceProvider = Ioc.Default;
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        serviceProvider.GetRequiredService<IMessenger>().Register(this);

        loadEventHandler = OnLoaded;
        documentTitleChangedEventHandler = OnDocumentTitleChanged;
        historyChangedEventHandler = OnHistoryChanged;

        Loaded += loadEventHandler;
    }

    public void Receive(UserChangedMessage message)
    {
        if (message.IsOnlyRoleChanged)
        {
            // Only role changed, we can't respond to this
            // since we only set selection locally.
            return;
        }

        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        taskContext.InvokeOnMainThread(RefreshWebview2Content);
    }

    [Command("GoBackCommand")]
    private void GoBack()
    {
        if (WebView.CoreWebView2.CanGoBack)
        {
            WebView.CoreWebView2.GoBack();
        }
    }

    [Command("RefreshCommand")]
    private void Refresh()
    {
        WebView.CoreWebView2.Reload();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitializeAsync().SafeForget();
    }

    private async ValueTask InitializeAsync()
    {
        if (!isInitializingOrInitialized)
        {
            isInitializingOrInitialized = true;

            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.DisableDevToolsForReleaseBuild();
            WebView.CoreWebView2.DocumentTitleChanged += documentTitleChangedEventHandler;
            WebView.CoreWebView2.HistoryChanged += historyChangedEventHandler;
        }

        RefreshWebview2Content();
    }

    private void OnDocumentTitleChanged(CoreWebView2 sender, object args)
    {
        DocumentTitle = sender.DocumentTitle;
    }

    private void OnHistoryChanged(CoreWebView2 sender, object args)
    {
        CanGoBack = sender.CanGoBack;
    }

    private async void RefreshWebview2Content()
    {
        User? user = serviceProvider.GetRequiredService<IUserService>().Current;
        if (user is null || user.SelectedUserGameRole is null)
        {
            return;
        }

        if (WebView.IsDisposed())
        {
            return;
        }

        CoreWebView2? coreWebView2 = WebView?.CoreWebView2;

        if (coreWebView2 is null)
        {
            return;
        }

        if (SourceProvider is null)
        {
            return;
        }

        if (!UserAndUid.TryFromUser(user, out UserAndUid? userAndUid))
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        string source = SourceProvider.GetSource(userAndUid);
        if (!string.IsNullOrEmpty(source))
        {
            CoreWebView2Navigator navigator = new(coreWebView2);
            await navigator.NavigateAsync("about:blank").ConfigureAwait(true);

            try
            {
                await coreWebView2.Profile.ClearBrowsingDataAsync();
            }
            catch (InvalidCastException)
            {
                infoBarService.Warning(SH.ViewControlWebViewerCoreWebView2ProfileQueryInterfaceFailed);
                await coreWebView2.DeleteCookiesAsync(userAndUid.IsOversea).ConfigureAwait(true);
            }

            coreWebView2
                .SetCookie(user.CookieToken, user.LToken, userAndUid.IsOversea)
                .SetMobileUserAgent(userAndUid.IsOversea);
            jsBridge?.Detach();
            jsBridge = SourceProvider.CreateJSBridge(serviceProvider, coreWebView2, userAndUid);

            await navigator.NavigateAsync(source).ConfigureAwait(true);
            await coreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.BrowsingHistory);
        }
    }
}