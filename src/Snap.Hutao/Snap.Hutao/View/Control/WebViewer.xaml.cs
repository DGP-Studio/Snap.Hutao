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
using System.Diagnostics;

namespace Snap.Hutao.View.Control;

[DependencyProperty("SourceProvider", typeof(IWebViewerSource))]
internal partial class WebViewer : UserControl, IRecipient<UserChangedMessage>
{
    private readonly IServiceProvider serviceProvider;
    private readonly RoutedEventHandler loadEventHandler;
    private readonly RoutedEventHandler unloadEventHandler;

    [SuppressMessage("", "IDE0052")]
    private MiHoYoJSInterface? jsInterface;

    public WebViewer()
    {
        InitializeComponent();
        serviceProvider = Ioc.Default;
        serviceProvider.GetRequiredService<IMessenger>().Register(this);

        loadEventHandler = OnLoaded;
        unloadEventHandler = OnUnloaded;

        Loaded += loadEventHandler;
        Unloaded += unloadEventHandler;
    }

    public void Receive(UserChangedMessage message)
    {
        if (message.NewValue?.SelectedUserGameRole is null)
        {
            return;
        }

        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        taskContext.InvokeOnMainThread(RefreshWebview2Content);
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

    private async void RefreshWebview2Content()
    {
        User? user = serviceProvider.GetRequiredService<IUserService>().Current;
        if (user is null)
        {
            return;
        }

        // TODO: replace with .NET 8 UnsafeAccessor
        try
        {
            CoreWebView2 coreWebView2 = WebView.CoreWebView2;

            if (SourceProvider is not null)
            {
                if (UserAndUid.TryFromUser(user, out UserAndUid? userAndUid))
                {
                    string source = SourceProvider.GetSource(userAndUid);
                    if (!string.IsNullOrEmpty(source))
                    {
                        foreach (CoreWebView2Cookie cookie in await coreWebView2.CookieManager.GetCookiesAsync(".mihoyo.com"))
                        {
                            coreWebView2.CookieManager.DeleteCookie(cookie);
                        }

                        coreWebView2.SetCookie(user.CookieToken, user.LToken, user.SToken).SetMobileUserAgent();
                        jsInterface = serviceProvider.CreateInstance<MiHoYoJSInterface>(coreWebView2, userAndUid);

                        CoreWebView2Navigator navigator = new(coreWebView2);
                        await navigator.NavigateAsync("about:blank").ConfigureAwait(true);
                        await navigator.NavigateAsync(source).ConfigureAwait(true);
                    }
                }
                else
                {
                    serviceProvider.GetRequiredService<IInfoBarService>().Warning(SH.MustSelectUserAndUid);
                }
            }
        }
        catch (ObjectDisposedException)
        {
        }
    }
}