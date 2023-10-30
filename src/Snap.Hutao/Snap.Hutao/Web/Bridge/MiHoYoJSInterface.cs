// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge.Model;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Response;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Foundation;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// 调用桥
/// </summary>
[HighQuality]
[SuppressMessage("", "CA1001")]
[SuppressMessage("", "CA1308")]
internal class MiHoYoJSInterface
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

    private readonly SemaphoreSlim webMessageSemaphore = new(1);
    private readonly Guid interfaceId = Guid.NewGuid();
    private readonly UserAndUid userAndUid;

    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly ILogger<MiHoYoJSInterface> logger;

    private readonly TypedEventHandler<CoreWebView2, CoreWebView2WebMessageReceivedEventArgs> webMessageReceivedEventHandler;
    private readonly TypedEventHandler<CoreWebView2, CoreWebView2DOMContentLoadedEventArgs> domContentLoadedEventHandler;
    private readonly TypedEventHandler<CoreWebView2, CoreWebView2NavigationStartingEventArgs> navigationStartingEventHandler;

    private CoreWebView2 coreWebView2;

    public MiHoYoJSInterface(CoreWebView2 coreWebView2, UserAndUid userAndUid)
    {
        // 由于Webview2 的作用域特殊性，我们在此处直接使用根服务
        serviceProvider = Ioc.Default;
        this.coreWebView2 = coreWebView2;
        this.userAndUid = userAndUid;

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        logger = serviceProvider.GetRequiredService<ILogger<MiHoYoJSInterface>>();

        webMessageReceivedEventHandler = OnWebMessageReceived;
        domContentLoadedEventHandler = OnDOMContentLoaded;
        navigationStartingEventHandler = OnNavigationStarting;

        coreWebView2.WebMessageReceived += webMessageReceivedEventHandler;
        coreWebView2.DOMContentLoaded += domContentLoadedEventHandler;
        coreWebView2.NavigationStarting += navigationStartingEventHandler;
    }

    public event Action? ClosePageRequested;

    /// <summary>
    /// 获取ActionTicket
    /// </summary>
    /// <param name="jsParam">参数</param>
    /// <returns>响应</returns>
    public virtual async ValueTask<IJsResult?> GetActionTicketAsync(JsParam<ActionTypePayload> jsParam)
    {
        return await serviceProvider
            .GetRequiredService<AuthClient>()
            .GetActionTicketBySTokenAsync(jsParam.Payload.ActionType, userAndUid.User)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// 获取Http请求头
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>Http请求头</returns>
    public virtual JsResult<Dictionary<string, string>> GetHttpRequestHeader(JsParam param)
    {
        return new()
        {
            Data = new Dictionary<string, string>()
            {
                { "x-rpc-client_type", "5" },
                { "x-rpc-device_id",  HoyolabOptions.DeviceId },
                { "x-rpc-app_version", SaltConstants.CNVersion },
            },
        };
    }

    /// <summary>
    /// 异步获取账户信息
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    public virtual JsResult<Dictionary<string, string>> GetCookieInfo(JsParam param)
    {
        ArgumentNullException.ThrowIfNull(userAndUid.User.LToken);

        return new()
        {
            Data = new()
            {
                [Cookie.LTUID] = userAndUid.User.LToken[Cookie.LTUID],
                [Cookie.LTOKEN] = userAndUid.User.LToken[Cookie.LTOKEN],
                [Cookie.LOGIN_TICKET] = string.Empty,
            },
        };
    }

    /// <summary>
    /// 获取1代动态密钥
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    public virtual JsResult<Dictionary<string, string>> GetDynamicSecrectV1(JsParam param)
    {
        string salt = HoyolabOptions.Salts[SaltType.LK2];
        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string r = GetRandomString();

        string check = Core.Convert.ToMd5HexString($"salt={salt}&t={t}&r={r}").ToLowerInvariant();

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
    /// 获取2代动态密钥
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    public virtual JsResult<Dictionary<string, string>> GetDynamicSecrectV2(JsParam<DynamicSecrect2Playload> param)
    {
        string salt = HoyolabOptions.Salts[SaltType.X4];
        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int r = GetRandom();
        string b = param.Payload.Body;
        string q = param.Payload.GetQueryParam();
        string check = Core.Convert.ToMd5HexString($"salt={salt}&t={t}&r={r}&b={b}&q={q}").ToLowerInvariant();

        return new() { Data = new() { ["DS"] = $"{t},{r},{check}" } };

        static int GetRandom()
        {
            int rand = Random.Shared.Next(100000, 200000);
            return rand == 100000 ? 642367 : rand;
        }
    }

    /// <summary>
    /// 获取用户基本信息
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    public virtual async ValueTask<JsResult<Dictionary<string, object>>> GetUserInfoAsync(JsParam param)
    {
        Response<UserFullInfoWrapper> response = await serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IUserClient>>()
            .Create(userAndUid.User.IsOversea)
            .GetUserFullInfoAsync(userAndUid.User)
            .ConfigureAwait(false);

        if (response.IsOk())
        {
            UserInfo info = response.Data.UserInfo;
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
        else
        {
            return new();
        }
    }

    /// <summary>
    /// 获取CookieToken
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    public virtual async ValueTask<JsResult<Dictionary<string, string>>> GetCookieTokenAsync(JsParam<CookieTokenPayload> param)
    {
        IUserService userService = serviceProvider.GetRequiredService<IUserService>();
        if (param.Payload.ForceRefresh)
        {
            await userService.RefreshCookieTokenAsync(userAndUid.User).ConfigureAwait(false);
        }

        await taskContext.SwitchToMainThreadAsync();
        coreWebView2.SetCookie(userAndUid.User.CookieToken, userAndUid.User.LToken);

        ArgumentNullException.ThrowIfNull(userAndUid.User.CookieToken);
        return new() { Data = new() { [Cookie.COOKIE_TOKEN] = userAndUid.User.CookieToken[Cookie.COOKIE_TOKEN] } };
    }

    /// <summary>
    /// 关闭
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    public virtual async ValueTask<IJsResult?> ClosePageAsync(JsParam param)
    {
        await taskContext.SwitchToMainThreadAsync();
        if (coreWebView2.CanGoBack)
        {
            coreWebView2.GoBack();
        }
        else
        {
            ClosePageRequested?.Invoke();
        }

        return null;
    }

    /// <summary>
    /// 调整分享设置
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>响应</returns>
    public virtual IJsResult? ConfigureShare(JsParam param)
    {
        return null;
    }

    /// <summary>
    /// 获取状态栏高度
    /// </summary>
    /// <param name="param">参数</param>
    /// <returns>结果</returns>
    public virtual JsResult<Dictionary<string, object>> GetStatusBarHeight(JsParam param)
    {
        return new() { Data = new() { ["statusBarHeight"] = 0 } };
    }

    public virtual async ValueTask<IJsResult?> PushPageAsync(JsParam<PushPagePayload> param)
    {
        await taskContext.SwitchToMainThreadAsync();
        coreWebView2.Navigate(param.Payload.Page);
        return null;
    }

    /// <summary>
    /// 获取当前语言和时区
    /// </summary>
    /// <param name="param">param</param>
    /// <returns>语言与时区</returns>
    public virtual JsResult<Dictionary<string, string>> GetCurrentLocale(JsParam<PushPagePayload> param)
    {
        MetadataOptions metadataOptions = serviceProvider.GetRequiredService<MetadataOptions>();

        return new()
        {
            Data = new()
            {
                ["language"] = metadataOptions.LanguageCode,
                ["timeZone"] = "GMT+8",
            },
        };
    }

    public virtual IJsResult? Share(JsParam<SharePayload> param)
    {
        return new JsResult<Dictionary<string, string>>()
        {
            Data = new()
            {
                ["type"] = param.Payload.Type,
            },
        };
    }

    public virtual ValueTask<IJsResult?> ShowAlertDialogAsync(JsParam param)
    {
        return ValueTask.FromException<IJsResult?>(new NotSupportedException());
    }

    public virtual IJsResult? StartRealPersonValidation(JsParam param)
    {
        throw new NotImplementedException();
    }

    public virtual IJsResult? StartRealnameAuth(JsParam param)
    {
        throw new NotImplementedException();
    }

    public virtual IJsResult? GenAuthKey(JsParam param)
    {
        throw new NotImplementedException();
    }

    public virtual IJsResult? GenAppAuthKey(JsParam param)
    {
        throw new NotImplementedException();
    }

    public virtual IJsResult? OpenSystemBrowser(JsParam param)
    {
        throw new NotImplementedException();
    }

    public virtual IJsResult? SaveLoginTicket(JsParam param)
    {
        throw new NotImplementedException();
    }

    public virtual ValueTask<IJsResult?> GetNotificationSettingsAsync(JsParam param)
    {
        throw new NotImplementedException();
    }

    public virtual IJsResult? ShowToast(JsParam param)
    {
        throw new NotImplementedException();
    }

    public void Detach()
    {
        coreWebView2.WebMessageReceived -= webMessageReceivedEventHandler;
        coreWebView2.DOMContentLoaded -= domContentLoadedEventHandler;
        coreWebView2.NavigationStarting -= navigationStartingEventHandler;
        coreWebView2 = default!;
    }

    private async ValueTask<string> ExecuteCallbackScriptAsync(string callback, string? payload = null)
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
            .AppendIf(!string.IsNullOrEmpty(payload), ',')
            .Append(payload)
            .Append(')')
            .ToString();

        logger?.LogInformation("[{Id}][ExecuteScript: {callback}]\n{payload}", interfaceId, callback, payload);

        await taskContext.SwitchToMainThreadAsync();
        try
        {
            if (coreWebView2 is not null)
            {
                return await coreWebView2.ExecuteScriptAsync(js);
            }
        }
        catch (COMException)
        {
            // COMException (0x8007139F): 组或资源的状态不是执行请求操作的正确状态。 (0x8007139F)
            // webview is disposing or disposed
        }

        return string.Empty;
    }

    private async void OnWebMessageReceived(CoreWebView2 webView2, CoreWebView2WebMessageReceivedEventArgs args)
    {
        string message = args.TryGetWebMessageAsString();
        logger.LogInformation("[{Id}][OnRawMessage]\n{message}", interfaceId, message);
        JsParam? param = JsonSerializer.Deserialize<JsParam>(message);

        ArgumentNullException.ThrowIfNull(param);
        logger.LogInformation("[OnMessage]\nMethod  : {method}\nPayload : {payload}\nCallback: {callback}", param.Method, param.Payload, param.Callback);
        using (await webMessageSemaphore.EnterAsync().ConfigureAwait(false))
        {
            IJsResult? result = await TryGetJsResultFromJsParamAsync(param).ConfigureAwait(false);

            if (result is not null && param.Callback is not null)
            {
                await ExecuteCallbackScriptAsync(param.Callback, result.ToJson()).ConfigureAwait(false);
            }
        }
    }

    [SuppressMessage("", "CA2254")]
    private IJsResult? LogUnhandledMessage([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] param)
    {
        logger.LogWarning(message, param);
        return default;
    }

    private async ValueTask<IJsResult?> TryGetJsResultFromJsParamAsync(JsParam param)
    {
        try
        {
            return param.Method switch
            {
                "closePage" => await ClosePageAsync(param).ConfigureAwait(false),
                "configure_share" => ConfigureShare(param),
                "eventTrack" => null,
                "getActionTicket" => await GetActionTicketAsync(param).ConfigureAwait(false),
                "getCookieInfo" => GetCookieInfo(param),
                "getCookieToken" => await GetCookieTokenAsync(param).ConfigureAwait(false),
                "getCurrentLocale" => GetCurrentLocale(param),
                "getDS" => GetDynamicSecrectV1(param),
                "getDS2" => GetDynamicSecrectV2(param),
                "getHTTPRequestHeaders" => GetHttpRequestHeader(param),
                "getStatusBarHeight" => GetStatusBarHeight(param),
                "getUserInfo" => await GetUserInfoAsync(param).ConfigureAwait(false),
                "hideLoading" => null,
                "login" => null,
                "pushPage" => await PushPageAsync(param).ConfigureAwait(false),
                "share" => Share(param),
                "showLoading" => null,
                _ => LogUnhandledMessage("Unhandled Message Type: {method}", param.Method),
            };
        }
        catch (ObjectDisposedException)
        {
            // The dialog is already closed.
            return null;
        }
    }

    private void OnDOMContentLoaded(CoreWebView2 coreWebView2, CoreWebView2DOMContentLoadedEventArgs args)
    {
        coreWebView2.ExecuteScriptAsync(HideScrollBarScript).AsTask().SafeForget(logger);
    }

    private void OnNavigationStarting(CoreWebView2 coreWebView2, CoreWebView2NavigationStartingEventArgs args)
    {
        string uriHost = new Uri(args.Uri).Host;
        ReadOnlySpan<char> uriHostSpan = uriHost.AsSpan();
        if (uriHostSpan.EndsWith("mihoyo.com") || uriHostSpan.EndsWith("hoyolab.com"))
        {
            // Execute this solve issue: When open same site second time,there might be no bridge init.
            // coreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(InitializeJsInterfaceScript2).AsTask().SafeForget(logger);
            coreWebView2.ExecuteScriptAsync(InitializeJsInterfaceScript2).AsTask().SafeForget(logger);
        }
    }
}