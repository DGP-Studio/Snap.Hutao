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
    [SuppressMessage("", "SA1310")]
    private static readonly Guid ICoreWebView2_13iid = Guid.Parse("314B5846-DBC7-5DE4-A792-647EA0F3296A");

    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly RoutedEventHandler loadEventHandler;
    private readonly RoutedEventHandler unloadEventHandler;

    private MiHoYoJSInterface? jsInterface;

    public WebViewer()
    {
        InitializeComponent();
        serviceProvider = Ioc.Default;
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        serviceProvider.GetRequiredService<IMessenger>().Register(this);

        loadEventHandler = OnLoaded;
        unloadEventHandler = OnUnloaded;

        Loaded += loadEventHandler;
        Unloaded += unloadEventHandler;
    }

    public void Receive(UserChangedMessage message)
    {
        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        taskContext.InvokeOnMainThread(RefreshWebview2Content);
    }

    private static bool IsCoreWebView2ProfileAvailable(CoreWebView2 coreWebView2)
    {
        int hr = ((IWinRTObject)coreWebView2).NativeObject.TryAs(ICoreWebView2_13iid, out ObjectReference<IUnknownVftbl> objRef);
        using (objRef)
        {
            if (hr >= 0)
            {
                // ICoreWebView2_13.Profile is available
                return true;
            }
        }

        return false;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitializeAsync().SafeForget();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= loadEventHandler;
        Unloaded -= unloadEventHandler;
    }

    private async ValueTask InitializeAsync()
    {
        await WebView.EnsureCoreWebView2Async();
        WebView.CoreWebView2.DisableDevToolsOnReleaseBuild();
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
                        if (IsCoreWebView2ProfileAvailable(coreWebView2))
                        {
                            await coreWebView2.Profile.ClearBrowsingDataAsync();
                        }
                        else
                        {
                            infoBarService.Warning(SH.ViewControlWebViewerCoreWebView2ProfileQueryInterfaceFailed);
                        }

                        CoreWebView2Navigator navigator = new(coreWebView2);
                        await navigator.NavigateAsync("about:blank").ConfigureAwait(true);

                        coreWebView2.SetCookie(user.CookieToken, user.LToken);
                        _ = userAndUid.User.IsOversea ? coreWebView2.SetMobileOverseaUserAgent() : coreWebView2.SetMobileUserAgent();
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