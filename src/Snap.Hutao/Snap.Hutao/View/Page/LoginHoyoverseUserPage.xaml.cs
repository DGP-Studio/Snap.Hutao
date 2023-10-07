// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 登录米哈游通行证页面
/// </summary>
internal sealed partial class LoginHoyoverseUserPage : Microsoft.UI.Xaml.Controls.Page, ISupportLoginByWebView
{
    private const string CookieHost = "https://account.hoyoverse.com";
    private const string NavigateUrl = "https://account.hoyoverse.com/#/login";

    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ILogger<LoginHoyoverseUserPage> logger;

    /// <summary>
    /// 构造一个新的登录米哈游通行证页面
    /// </summary>
    public LoginHoyoverseUserPage()
    {
        serviceProvider = Ioc.Default;
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        logger = serviceProvider.GetRequiredService<ILogger<LoginHoyoverseUserPage>>();

        InitializeComponent();
    }

    private async ValueTask<string> GetUidFromCookieAsync(Cookie cookie, CancellationToken token = default)
    {
        IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory = serviceProvider.GetRequiredService<IHttpRequestMessageBuilderFactory>();
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.WebApiOsAccountLoginByCookie)
            .SetHeader("Cookie", cookie.ToString());

        HttpClient httpClient = serviceProvider.GetRequiredService<HttpClient>();

        WebApiResponse<AccountInfoWrapper>? resp = await builder
            .TryCatchSendAsync<WebApiResponse<AccountInfoWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return $"{resp?.Data?.AccountInfo?.AccountId}";
    }

    private void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        ISupportLoginByWebView.InitialzeAsync(WebView, infoBarService, CookieHost, NavigateUrl).SafeForget();
    }

    [Command("HandleCurrentCookieCommand")]
    private async Task HandleCurrentCookieAsync()
    {
        IReadOnlyList<CoreWebView2Cookie> cookies = await WebView.CoreWebView2.CookieManager.GetCookiesAsync("https://account.hoyoverse.com");
        Cookie loginTicketCookie = Cookie.FromCoreWebView2Cookies(cookies);

        if (loginTicketCookie.IsEmpty())
        {
            return;
        }

        string uid = await GetUidFromCookieAsync(loginTicketCookie).ConfigureAwait(false);
        loginTicketCookie[Cookie.LOGIN_UID] = uid;

        // 使用 loginTicket 获取 stoken
        Response<ListWrapper<NameToken>> multiTokenResponse = await serviceProvider
            .GetRequiredService<AuthClient>()
            .GetMultiTokenByLoginTicketAsync(loginTicketCookie, true)
            .ConfigureAwait(false);

        if (!multiTokenResponse.IsOk())
        {
            return;
        }

        Dictionary<string, string> multiTokenMap = multiTokenResponse.Data.List.ToDictionary(n => n.Name, n => n.Token);
        Cookie stokenV1 = Cookie.FromSToken(uid, multiTokenMap[Cookie.STOKEN]);

        await ISupportLoginByWebView
            .PostHandleCurrentCookieAsync(serviceProvider, stokenV1, true)
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
