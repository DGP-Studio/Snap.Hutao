// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Request;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 登录米哈游通行证页面
/// </summary>
internal sealed partial class LoginHoyoverseUserPage : Microsoft.UI.Xaml.Controls.Page
{
    /// <summary>
    /// 构造一个新的登录米哈游通行证页面
    /// </summary>
    public LoginHoyoverseUserPage()
    {
        InitializeComponent();
    }

    private static async ValueTask<string> GetUidFromCookieAsync(IServiceProvider serviceProvider, Cookie cookie, CancellationToken token = default)
    {
        JsonSerializerOptions options = serviceProvider.GetRequiredService<JsonSerializerOptions>();
        ILogger<LoginHoyoverseUserPage> logger = serviceProvider.GetRequiredService<ILogger<LoginHoyoverseUserPage>>();
        HttpClient httpClient = serviceProvider.GetRequiredService<HttpClient>();

        httpClient.DefaultRequestHeaders.Set("Cookie", cookie.ToString());

        WebApiResponse<AccountInfoWrapper>? resp = await httpClient
            .TryCatchGetFromJsonAsync<WebApiResponse<AccountInfoWrapper>>(ApiOsEndpoints.WebApiOsAccountLoginByCookie, options, logger, token)
            .ConfigureAwait(false);

        if (resp is not null)
        {
            return $"{resp.Data.AccountInfo.AccountId}";
        }

        return string.Empty;
    }

    private async void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await WebView.EnsureCoreWebView2Async();

            CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
            IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://account.hoyoverse.com");
            foreach (CoreWebView2Cookie item in cookies)
            {
                manager.DeleteCookie(item);
            }

            CoreWebView2 coreWebView2 = WebView.CoreWebView2;
            coreWebView2.Settings.IsGeneralAutofillEnabled = false;
            coreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            coreWebView2.Navigate("https://account.hoyoverse.com/#/login");
        }
        catch (Exception ex)
        {
            Ioc.Default.GetRequiredService<IInfoBarService>().Error(ex);
        }
    }

    [Command("HandleCurrentCookieCommand")]
    private async Task HandleCurrentCookieAsync()
    {
        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://account.hoyoverse.com");

        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();

        Cookie loginTicketCookie = Cookie.FromCoreWebView2Cookies(cookies);
        string uid = await GetUidFromCookieAsync(Ioc.Default, loginTicketCookie).ConfigureAwait(false);
        loginTicketCookie[Cookie.LOGIN_UID] = uid;

        // 使用 loginTicket 获取 stoken
        Response<ListWrapper<NameToken>> multiTokenResponse = await Ioc.Default
            .GetRequiredService<AuthClient>()
            .GetMultiTokenByLoginTicketAsync(loginTicketCookie, true)
            .ConfigureAwait(false);

        if (!multiTokenResponse.IsOk())
        {
            return;
        }

        Dictionary<string, string> multiTokenMap = multiTokenResponse.Data.List.ToDictionary(n => n.Name, n => n.Token);
        Cookie hoyolabCookie = Cookie.FromSToken(uid, multiTokenMap[Cookie.STOKEN]);

        // 处理 cookie 并添加用户
        (UserOptionResult result, string nickname) = await Ioc.Default
            .GetRequiredService<IUserService>()
            .ProcessInputCookieAsync(hoyolabCookie, true)
            .ConfigureAwait(false);

        Ioc.Default.GetRequiredService<INavigationService>().GoBack();

        await Ioc.Default
            .GetRequiredService<ViewModel.User.UserViewModel>()
            .HandleUserOptionResultAsync(result, nickname)
            .ConfigureAwait(false);
    }

    private sealed class WebApiResponse<TData>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("data")]
        public TData Data { get; set; } = default!;
    }

    private sealed class AccountInfoWrapper
    {
        [JsonPropertyName("account_info")]
        public AccountInfo AccountInfo { get; set; } = default!;

        [JsonPropertyName("game_ctrl_info")]
        public JsonElement GameControlInfo { get; set; }

        [JsonPropertyName("info")]
        public string Info { get; set; } = default!;

        [JsonPropertyName("msg")]
        public string Message { get; set; } = default!;

        [JsonPropertyName("notice_info")]
        public JsonElement NoticeInfo { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }

    private sealed class AccountInfo
    {
        [JsonPropertyName("account_id")]
        public int AccountId { get; set; }

        [JsonPropertyName("account_name")]
        public string AccountName { get; set; } = default!;

        [JsonPropertyName("area_code")]
        public string AreaCode { get; set; } = default!;

        [JsonPropertyName("country")]
        public string Country { get; set; } = default!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("mobile")]
        public string Mobile { get; set; } = default!;

        [JsonPropertyName("safe_level")]
        public int SafeLevel { get; set; }

        [JsonPropertyName("weblogin_token")]
        public string WebLoginToken { get; set; } = default!;
    }
}
