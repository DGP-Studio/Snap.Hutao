// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Convert;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Bridge.Model;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using System.Text;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// 调用桥
/// </summary>
public class MiHoYoJSInterface
{
    private const string InitializeJsInterfaceScript2 = """
        window.MiHoYoJSInterface = {
            postMessage: function(arg) { chrome.webview.postMessage(arg) },
            closePage: function() { this.postMessage('{"method":"closePage"}') },
        };
        """;

    private const string HideScrollBarScript = """
        let st = document.createElement('style');
        st.innerHTML = '::-webkit-scrollbar{display:none}';
        document.querySelector('body').appendChild(st);
        """;

    private readonly CoreWebView2 webView;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<MiHoYoJSInterface> logger;
    private readonly JsonSerializerOptions options;

    /// <summary>
    /// 构造一个新的调用桥
    /// </summary>
    /// <param name="webView">webview2</param>
    /// <param name="serviceProvider">服务提供器</param>
    protected MiHoYoJSInterface(CoreWebView2 webView, IServiceProvider serviceProvider)
    {
        this.webView = webView;
        this.serviceProvider = serviceProvider;

        logger = serviceProvider.GetRequiredService<ILogger<MiHoYoJSInterface>>();
        options = serviceProvider.GetRequiredService<JsonSerializerOptions>();

        webView.WebMessageReceived += OnWebMessageReceived;
        webView.DOMContentLoaded += OnDOMContentLoaded;
        webView.NavigationStarting += OnNavigationStarting;
    }

    /// <summary>
    /// 获取ActionTicket
    /// </summary>
    /// <param name="jsParam">参数</param>
    /// <returns>响应</returns>
    [JsMethod("getActionTicket")]
    public virtual async Task<IJsResult?> GetActionTicketAsync(JsParam<ActionTypePayload> jsParam)
    {
        User user = serviceProvider.GetRequiredService<IUserService>().Current!;
        return await serviceProvider
            .GetRequiredService<AuthClient>()
            .GetActionTicketWrapperByStokenAsync(jsParam.Payload!.ActionType, user.Entity)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// 获取Http请求头
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>Http请求头</returns>
    [JsMethod("getHTTPRequestHeaders")]
    public virtual JsResult<Dictionary<string, string>> GetHttpRequestHeader(JsParam param)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 异步获取账户信息
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    [JsMethod("getCookieInfo")]
    public virtual JsResult<Dictionary<string, string>> GetCookieInfo(JsParam param)
    {
        User user = serviceProvider.GetRequiredService<IUserService>().Current!;

        return new()
        {
            Data = new()
            {
                [Cookie.LTUID] = user.Ltoken![Cookie.LTUID],
                [Cookie.LTOKEN] = user.Ltoken[Cookie.LTOKEN],
                [Cookie.LOGIN_TICKET] = string.Empty,
            },
        };
    }

    /// <summary>
    /// 获取1代动态密钥
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    [JsMethod("getDS")]
    public virtual JsResult<Dictionary<string, string>> GetDynamicSecrectV1(JsParam param)
    {
        string salt = DynamicSecretHandler.DynamicSecrets[nameof(SaltType.LK2)];
        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string r = GetRandomString();
        string check = Md5Convert.ToHexString($"salt={salt}&t={t}&r={r}").ToLowerInvariant();

        return new() { Data = new() { ["DS"] = $"{t},{r},{check}", }, };

        static string GetRandomString()
        {
            const string RandomRange = "abcdefghijklmnopqrstuvwxyz1234567890";

            StringBuilder sb = new(6);

            for (int i = 0; i < 6; i++)
            {
                int pos = Random.Shared.Next(0, RandomRange.Length);
                sb.Append(RandomRange[pos]);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// 获取用户基本信息
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    [JsMethod("getUserInfo")]
    public virtual JsResult<Dictionary<string, object>> GetUserInfo(JsParam param)
    {
        User user = serviceProvider.GetRequiredService<IUserService>().Current!;
        UserInfo info = user.UserInfo!;

        return new()
        {
            Data = new()
            {
                ["id"] = info.Uid,
                ["gender"] = info.Gender,
                ["nickname"] = info.Nickname,
                ["introduce"] = info.Introduce,
                ["avatar_url"] = info.AvatarUrl,
            },
        };
    }

    [JsMethod("getCookieToken")]
    public virtual async Task<JsResult<Dictionary<string, string>>> GetCookieTokenAsync(JsParam<CookieTokenPayload> param)
    {
        User user = serviceProvider.GetRequiredService<IUserService>().Current!;
        string? cookieToken;
        if (param.Payload!.ForceRefresh)
        {
            cookieToken = await Ioc.Default
                .GetRequiredService<PassportClient2>()
                .GetCookieAccountInfoBySTokenAsync(user.Entity, default)
                .ConfigureAwait(false);

            user.CookieToken![Cookie.COOKIE_TOKEN] = cookieToken!;
            Ioc.Default.GetRequiredService<AppDbContext>().Users.UpdateAndSave(user.Entity);
        }
        else
        {
            cookieToken = user.CookieToken![Cookie.COOKIE_TOKEN];
        }

        return new() { Data = new() { [Cookie.COOKIE_TOKEN] = cookieToken! } };
    }

    [JsMethod("configure_share")]
    public virtual Task<IJsResult?> ConfigureShare(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("showAlertDialog")]
    public virtual Task<IJsResult?> ShowAlertDialogAsync(JsParam param)
    {
        return Task.FromException<IJsResult?>(new NotImplementedException());
    }

    [JsMethod("closePage")]
    public virtual IJsResult? ClosePage(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("startRealPersonValidation")]
    public virtual IJsResult? StartRealPersonValidation(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("startRealnameAuth")]
    public virtual IJsResult? StartRealnameAuth(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("getDS2")]
    public virtual IJsResult? GetDynamicSecrectV2(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("genAuthKey")]
    public virtual IJsResult? GenAuthKey(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("genAppAuthKey")]
    public virtual IJsResult? GenAppAuthKey(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("getStatusBarHeight")]
    public virtual IJsResult? GetStatusBarHeight(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("pushPage")]
    public virtual IJsResult? PushPage(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("openSystemBrowser")]
    public virtual IJsResult? OpenSystemBrowser(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("saveLoginTicket")]
    public virtual IJsResult? SaveLoginTicket(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("getNotificationSettings")]
    public virtual Task<IJsResult?> GetNotificationSettingsAsync(JsParam param)
    {
        throw new NotImplementedException();
    }

    [JsMethod("showToast")]
    public virtual Task<IJsResult?> ShowToast(JsParam param)
    {
        throw new NotImplementedException();
    }

    private async Task<string> ExecuteCallbackScriptAsync(string callback, string? payload = null)
    {
        if (string.IsNullOrEmpty(callback))
        {
            // prevent executing this callback
            return string.Empty;
        }

        string js = new StringBuilder()
            .Append("javascript:mhyWebBridge(")
            .Append('"')
            .Append(callback)
            .Append('"')
            .AppendIf(payload != null, ',')
            .Append(payload)
            .Append(')')
            .ToString();

        logger?.LogInformation("[ExecuteScript] {js}", js);

        await ThreadHelper.SwitchToMainThreadAsync();
        return await webView.ExecuteScriptAsync(js);
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnWebMessageReceived(CoreWebView2 webView2, CoreWebView2WebMessageReceivedEventArgs args)
    {
        string message = args.TryGetWebMessageAsString();
        logger?.LogInformation("[OnMessage] {message}", message);

        JsParam param = JsonSerializer.Deserialize<JsParam>(message)!;

        IJsResult? result = param.Method switch
        {
            "getActionTicket" => await GetActionTicketAsync(param).ConfigureAwait(false),
            "getHTTPRequestHeaders" => GetHttpRequestHeader(param),
            "getCookieInfo" => GetCookieInfo(param),
            "getDS" => GetDynamicSecrectV1(param),
            "getUserInfo" => GetUserInfo(param),
            "getCookieToken" => await GetCookieTokenAsync(param).ConfigureAwait(false),
            "configure_share" => null,
            "login" => null,
            _ => null,
        };

        if (result != null)
        {
            await ExecuteCallbackScriptAsync(param.Callback, result.ToString(options)).ConfigureAwait(false);
        }
    }

    private void OnDOMContentLoaded(CoreWebView2 coreWebView2, CoreWebView2DOMContentLoadedEventArgs args)
    {
        coreWebView2.ExecuteScriptAsync(HideScrollBarScript).AsTask().SafeForget(logger);
    }

    private void OnNavigationStarting(CoreWebView2 coreWebView2, CoreWebView2NavigationStartingEventArgs args)
    {
        if (new Uri(args.Uri).Host.EndsWith("mihoyo.com"))
        {
            coreWebView2.ExecuteScriptAsync(InitializeJsInterfaceScript2).AsTask().SafeForget(logger);
        }
    }
}