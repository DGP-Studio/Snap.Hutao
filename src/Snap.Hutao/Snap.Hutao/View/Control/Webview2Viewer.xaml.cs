// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.View.Control;

[DependencyProperty("Source", typeof(string), default(string)!, nameof(OnSourceChanged))]
[DependencyProperty("User", typeof(User))]
internal partial class Webview2Viewer : UserControl
{
    private readonly IServiceProvider serviceProvider;
    private readonly RoutedEventHandler loadEventHandler;
    private readonly RoutedEventHandler unloadEventHandler;

    private MiHoYoJSInterface? jsInterface;
    private bool isInitialized;

    public Webview2Viewer()
    {
        InitializeComponent();
        serviceProvider = Ioc.Default;

        loadEventHandler = OnLoaded;
        unloadEventHandler = OnUnloaded;

        Loaded += loadEventHandler;
        Unloaded += unloadEventHandler;
    }

    private static void OnSourceChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        Webview2Viewer viewer = (Webview2Viewer)dp;
        if (viewer.isInitialized)
        {
            viewer.RefreshWebview2Content();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitializeAsync().SafeForget();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        jsInterface = null;
        Loaded -= loadEventHandler;
        Unloaded -= unloadEventHandler;
    }

    private async ValueTask InitializeAsync()
    {
        await WebView.EnsureCoreWebView2Async();
        RefreshWebview2Content();
    }

    private void RefreshWebview2Content()
    {
        if (User is null)
        {
            IUserService userService = serviceProvider.GetRequiredService<IUserService>();
            User ??= userService.Current;
            if (User is null)
            {
                return;
            }
        }

        CoreWebView2 coreWebView2 = WebView.CoreWebView2;
        if (UserAndUid.TryFromUser(User, out UserAndUid? userAndUid) && !string.IsNullOrEmpty(Source))
        {
            coreWebView2.SetCookie(User.CookieToken, User.LToken, User.SToken).SetMobileUserAgent();
            jsInterface = serviceProvider.CreateInstance<MiHoYoJSInterface>(coreWebView2, userAndUid);
            coreWebView2.Navigate(Source);
        }
        else
        {
            serviceProvider.GetRequiredService<IInfoBarService>().Warning(SH.MustSelectUserAndUid);
        }

        isInitialized = true;
    }
}
