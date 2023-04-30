// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 养成计算器对话框
/// </summary>
[HighQuality]
internal sealed partial class AdoptCalculatorDialog : ContentDialog
{
    private readonly ITaskContext taskContext;
    private readonly IServiceScope scope;
    private MiHoYoJSInterface? jsInterface;

    /// <summary>
    /// 构造一个新的养成计算器对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public AdoptCalculatorDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        XamlRoot = serviceProvider.GetRequiredService<MainWindow>().Content.XamlRoot;

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        scope = serviceProvider.CreateScope();
    }

    private void OnGridLoaded(object sender, RoutedEventArgs e)
    {
        InitializeAsync().SafeForget();
    }

    private async Task InitializeAsync()
    {
        await WebView.EnsureCoreWebView2Async();
        CoreWebView2 coreWebView2 = WebView.CoreWebView2;

        IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        User? user = userService.Current;

        if (user == null)
        {
            return;
        }

        coreWebView2.SetCookie(user.CookieToken, user.LToken, null).SetMobileUserAgent();
        jsInterface = new(coreWebView2, scope.ServiceProvider);
        jsInterface.ClosePageRequested += OnClosePageRequested;

        coreWebView2.Navigate($"http://webstatic.mihoyo.com/ys/event/e20200923adopt_calculator/index.html?bbs_presentation_style=fullscreen&bbs_auth_required=true&&utm_source=bbs&utm_medium=mys&utm_campaign=GameRecord");
    }

    private void OnClosePageRequested()
    {
        taskContext.InvokeOnMainThread(Hide);
    }

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        jsInterface = null;
        scope.Dispose();
    }
}
