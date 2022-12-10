// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 养成计算器对话框
/// </summary>
public sealed partial class AdoptCalculatorDialog : ContentDialog
{
    private readonly IServiceScope scope;
    [SuppressMessage("", "IDE0052")]
    private CalculatorJsInterface? dailyNoteJsInterface;

    /// <summary>
    /// 构造一个新的养成计算器对话框
    /// </summary>
    /// <param name="window">窗体</param>
    public AdoptCalculatorDialog(Window window)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
        scope = Ioc.Default.CreateScope();
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

        coreWebView2.SetCookie(user.CookieToken, user.Ltoken, null).SetMobileUserAgent();
        dailyNoteJsInterface = new(coreWebView2, scope.ServiceProvider);

#if DEBUG
        coreWebView2.OpenDevToolsWindow();
#endif
        coreWebView2.Navigate($"http://webstatic.mihoyo.com/ys/event/e20200923adopt_calculator/index.html?bbs_presentation_style=fullscreen&bbs_auth_required=true&&utm_source=bbs&utm_medium=mys&utm_campaign=GameRecord");
    }

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        dailyNoteJsInterface = null;
        scope.Dispose();
    }
}
