// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Message;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.View.Control;

[DependencyProperty("SourceProvider", typeof(IWebViewerSource))]
internal partial class WebViewer : UserControl, IRecipient<UserChangedMessage>
{
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly RoutedEventHandler loadEventHandler;

    private MiHoYoJSInterface? jsInterface;
    private bool isInitializingOrInitialized;

    public WebViewer()
    {
        Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00000000");
        InitializeComponent();
        serviceProvider = Ioc.Default;
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        serviceProvider.GetRequiredService<IMessenger>().Register(this);

        loadEventHandler = OnLoaded;

        Loaded += loadEventHandler;
    }

    public void Receive(UserChangedMessage message)
    {
        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        taskContext.InvokeOnMainThread(RefreshWebview2Content);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitializeAsync().SafeForget();
    }

    private async ValueTask InitializeAsync()
    {
        if (isInitializingOrInitialized)
        {
            return;
        }

        isInitializingOrInitialized = true;

        await WebView.EnsureCoreWebView2Async();
        WebView.CoreWebView2.DisableDevToolsForReleaseBuild();
        RefreshWebview2Content();
    }

    private async void RefreshWebview2Content()
    {
        User? user = serviceProvider.GetRequiredService<IUserService>().Current;
        if (user is null || user.SelectedUserGameRole is null)
        {
            return;
        }

        // TODO: replace with .NET 8 UnsafeAccessor
        try
        {
            CoreWebView2? coreWebView2 = WebView?.CoreWebView2;

            if (coreWebView2 is null)
            {
                return;
            }

            if (SourceProvider is not null)
            {
                if (UserAndUid.TryFromUser(user, out UserAndUid? userAndUid))
                {
                    string source = SourceProvider.GetSource(userAndUid);
                    if (!string.IsNullOrEmpty(source))
                    {
                        try
                        {
                            await coreWebView2.Profile.ClearBrowsingDataAsync();
                        }
                        catch (InvalidCastException)
                        {
                            infoBarService.Warning(SH.ViewControlWebViewerCoreWebView2ProfileQueryInterfaceFailed);
                            await coreWebView2.DeleteCookiesAsync(userAndUid.IsOversea).ConfigureAwait(true);
                        }

                        CoreWebView2Navigator navigator = new(coreWebView2);
                        await navigator.NavigateAsync("about:blank").ConfigureAwait(true);

                        coreWebView2
                            .SetCookie(user.CookieToken, user.LToken, userAndUid.IsOversea)
                            .SetMobileUserAgent(userAndUid.IsOversea);
                        jsInterface?.Detach();
                        jsInterface = SourceProvider.CreateJsInterface(serviceProvider, coreWebView2, userAndUid);

                        await navigator.NavigateAsync(source).ConfigureAwait(true);
                    }
                }
                else
                {
                    infoBarService.Warning(SH.MustSelectUserAndUid);
                }
            }
        }
        catch (ObjectDisposedException)
        {
        }
    }
}