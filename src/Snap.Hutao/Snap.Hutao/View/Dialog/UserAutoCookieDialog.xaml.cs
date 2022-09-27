// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Threading;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 用户自动Cookie对话框
/// </summary>
public sealed partial class UserAutoCookieDialog : ContentDialog
{
    private IDictionary<string, string>? cookie;

    /// <summary>
    /// 构造一个新的用户自动Cookie对话框
    /// </summary>
    /// <param name="window">依赖窗口</param>
    public UserAutoCookieDialog(Window window)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
    }

    /// <summary>
    /// 获取输入的Cookie
    /// </summary>
    /// <returns>输入的结果</returns>
    public async Task<ValueResult<bool, IDictionary<string, string>>> GetInputCookieAsync()
    {
        ContentDialogResult result = await ShowAsync();
        return new(result == ContentDialogResult.Primary && cookie != null, cookie!);
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        await WebView.EnsureCoreWebView2Async();
        WebView.CoreWebView2.SourceChanged += OnCoreWebView2SourceChanged;

        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://user.mihoyo.com");
        foreach (var item in cookies)
        {
            manager.DeleteCookie(item);
        }

        WebView.CoreWebView2.Navigate("https://user.mihoyo.com/#/login/password");
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnCoreWebView2SourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
    {
        if (sender != null)
        {
            if (sender.Source.ToString() == "https://user.mihoyo.com/#/account/home")
            {
                try
                {
                    CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
                    IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://user.mihoyo.com");
                    cookie = cookies.ToDictionary(c => c.Name, c => c.Value);
                    WebView.CoreWebView2.SourceChanged -= OnCoreWebView2SourceChanged;
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
