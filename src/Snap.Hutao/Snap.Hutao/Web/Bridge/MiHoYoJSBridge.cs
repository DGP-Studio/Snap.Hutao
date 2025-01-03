// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge.Model;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Bridge;

internal class MiHoYoJSBridge
{
    private const string InitializeJsInterfaceScript = """
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

    private const string ConvertMouseToTouchScript = """
        function mouseListener (e, event) {
            let touch = new Touch({
                identifier: Date.now(),
                target: e.target,
                clientX: e.clientX,
                clientY: e.clientY,
                screenX: e.screenX,
                screenY: e.screenY,
                pageX: e.pageX,
                pageY: e.pageY,
            });
            let touchEvent = new TouchEvent(event, {
                cancelable: true,
                bubbles: true,
                touches: [touch],
                targetTouches: [touch],
                changedTouches: [touch],
            });
            e.target.dispatchEvent(touchEvent);
        }

        let mouseMoveListener = (e) => {
            mouseListener(e, 'touchmove'); 
        };

        let mouseUpListener = (e) => {
            mouseListener(e, 'touchend'); 
            document.removeEventListener('mousemove', mouseMoveListener);
            document.removeEventListener('mouseup', mouseUpListener);
        };

        let mouseDownListener = (e) => {
            mouseListener(e, 'touchstart'); 
            document.addEventListener('mousemove', mouseMoveListener);
            document.addEventListener('mouseup', mouseUpListener);
        };
        document.addEventListener('mousedown', mouseDownListener);
        """;

    private readonly AsyncLock webMessageLock = new();
    private readonly Guid bridgeId = Guid.NewGuid();
    private readonly UserAndUid userAndUid;

    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly ILogger<MiHoYoJSBridge> logger;

    private CoreWebView2 coreWebView2;

    public MiHoYoJSBridge(IServiceProvider serviceProvider, CoreWebView2 coreWebView2, UserAndUid userAndUid)
    {
        this.serviceProvider = serviceProvider;

        this.coreWebView2 = coreWebView2;
        this.userAndUid = userAndUid;

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        logger = serviceProvider.GetRequiredService<ILogger<MiHoYoJSBridge>>();

        coreWebView2.WebMessageReceived += OnWebMessageReceived;
        coreWebView2.DOMContentLoaded += OnDOMContentLoaded;
        coreWebView2.NavigationStarting += OnNavigationStarting;
    }

    public event Action? ClosePageRequested;

    protected ILogger Logger { get => logger; }

    public void Detach()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (coreWebView2 is null)
        {
            return;
        }

        coreWebView2.WebMessageReceived -= OnWebMessageReceived;
        coreWebView2.DOMContentLoaded -= OnDOMContentLoaded;
        coreWebView2.NavigationStarting -= OnNavigationStarting;
        coreWebView2 = default!;
    }

    protected virtual async ValueTask<IJsBridgeResult?> ClosePageAsync(JsParam param)
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

    protected virtual IJsBridgeResult? ConfigureShare(JsParam param)
    {
        return null;
    }

    protected virtual async ValueTask<IJsBridgeResult?> GetActionTicketAsync(JsParam<ActionTypePayload> jsParam)
    {
        return await serviceProvider
            .GetRequiredService<AuthClient>()
            .GetActionTicketBySTokenAsync(jsParam.Payload.ActionType, userAndUid.User)
            .ConfigureAwait(false);
    }

    protected virtual JsResult<Dictionary<string, string>> GetCookieInfo(JsParam param)
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

    protected virtual async ValueTask<JsResult<Dictionary<string, string>>> GetCookieTokenAsync(JsParam<CookieTokenPayload> param)
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

    protected virtual JsResult<Dictionary<string, string>> GetCurrentLocale(JsParam<PushPagePayload> param)
    {
        CultureOptions cultureOptions = serviceProvider.GetRequiredService<CultureOptions>();
        TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);

        return new()
        {
            Data = new()
            {
                ["language"] = cultureOptions.LanguageCode,
                ["timeZone"] = $"GMT{(offset.Hours >= 0 ? "+" : " - ")}{Math.Abs(offset.Hours):D1}",
            },
        };
    }

    protected virtual JsResult<Dictionary<string, string>> GetDataSignV1(JsParam param)
    {
        DataSignOptions options = DataSignOptions.CreateForGeneration1(SaltType.LK2, true);
        return new()
        {
            Data = new()
            {
                ["DS"] = DataSignAlgorithm.GetDataSign(options),
            },
        };
    }

    protected virtual JsResult<Dictionary<string, string>> GetDataSignV2(JsParam<DataSignV2Payload> param)
    {
        DataSignOptions options = DataSignOptions.CreateForGeneration2(SaltType.X4, false, param.Payload.Body, param.Payload.GetQueryParam());
        return new()
        {
            Data = new()
            {
                ["DS"] = DataSignAlgorithm.GetDataSign(options),
            },
        };
    }

    protected virtual JsResult<Dictionary<string, string>> GetHttpRequestHeader(JsParam param)
    {
        Dictionary<string, string> headers = new()
        {
            // Skip x-rpc-device_name
            // Skip x-rpc-device_model
            // Skip x-rpc-sys_version
            // Skip x-rpc-game_biz
            // Skip x-rpc-lifecycle_id
            { "x-rpc-app_id", "bll8iq97cem8" },
            { "x-rpc-client_type", "5" },
            { "x-rpc-device_id", HoyolabOptions.DeviceId36 },
            { "x-rpc-app_version", userAndUid.IsOversea ? SaltConstants.OSVersion : SaltConstants.CNVersion },
            { "x-rpc-sdk_version", "2.16.0" },
        };

        if (!userAndUid.IsOversea)
        {
            headers.Add("x-rpc-device_fp", userAndUid.User.Fingerprint ?? string.Empty);
        }

        GetHttpRequestHeaderCore(headers);

        return new()
        {
            Data = headers,
        };
    }

    protected virtual void GetHttpRequestHeaderCore(Dictionary<string, string> headers)
    {
    }

    protected virtual JsResult<Dictionary<string, object>> GetStatusBarHeight(JsParam param)
    {
        return new() { Data = new() { ["statusBarHeight"] = 0 } };
    }

    protected virtual async ValueTask<JsResult<Dictionary<string, object>>> GetUserInfoAsync(JsParam param)
    {
        UserFullInfoWrapper? wrapper;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IUserClient userClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IUserClient>>()
                .Create(userAndUid.IsOversea);

            Response<UserFullInfoWrapper> response = await userClient
                .GetUserFullInfoAsync(userAndUid.User)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out wrapper))
            {
                return new();
            }
        }

        UserInfo info = wrapper.UserInfo;
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

    protected virtual async ValueTask<IJsBridgeResult?> PushPageAsync(JsParam<PushPagePayload> param)
    {
        const string BbsSchema = "mihoyobbs://";
        string pageUrl = param.Payload.Page;

        string targetUrl = pageUrl;
        if (pageUrl.AsSpan().StartsWith(BbsSchema, StringComparison.OrdinalIgnoreCase))
        {
            if (pageUrl.AsSpan(BbsSchema.Length).StartsWith("article/"))
            {
                targetUrl = pageUrl.Replace("mihoyobbs://article/", "https://m.miyoushe.com/ys/#/article/", StringComparison.OrdinalIgnoreCase);
            }
            else if (pageUrl.AsSpan(BbsSchema.Length).StartsWith("webview?link="))
            {
                string encoded = pageUrl.Replace("mihoyobbs://webview?link=", string.Empty, StringComparison.OrdinalIgnoreCase);
                targetUrl = Uri.UnescapeDataString(encoded);
            }
        }

        await taskContext.SwitchToMainThreadAsync();
        coreWebView2.Navigate(targetUrl);
        return null;
    }

    protected virtual async ValueTask<IJsBridgeResult?> ShareAsync(JsParam<SharePayload> param)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            JsonSerializerOptions jsonSerializerOptions = scope.ServiceProvider.GetRequiredService<JsonSerializerOptions>();
            HttpClient httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            IClipboardProvider clipboardProvider = scope.ServiceProvider.GetRequiredService<IClipboardProvider>();
            IInfoBarService infoBarService = scope.ServiceProvider.GetRequiredService<IInfoBarService>();
            IFileSystemPickerInteraction fileSystemPickerInteraction = scope.ServiceProvider.GetRequiredService<IFileSystemPickerInteraction>();
            BridgeShareSaveType shareSaveType = scope.ServiceProvider.GetRequiredService<AppOptions>().BridgeShareSaveType;

            BridgeShareContext context = new(coreWebView2, taskContext, httpClient, infoBarService, clipboardProvider, jsonSerializerOptions, fileSystemPickerInteraction, shareSaveType);

            return await BridgeShareImplementation.ShareAsync(param, context).ConfigureAwait(false);
        }
    }

    protected virtual ValueTask<IJsBridgeResult?> ShowAlertDialogAsync(JsParam param)
    {
        return ValueTask.FromException<IJsBridgeResult?>(new NotSupportedException());
    }

    protected virtual IJsBridgeResult? StartRealPersonValidation(JsParam param)
    {
        throw new NotImplementedException();
    }

    protected virtual IJsBridgeResult? StartRealnameAuth(JsParam param)
    {
        throw new NotImplementedException();
    }

    protected virtual IJsBridgeResult? GenAuthKey(JsParam param)
    {
        throw new NotImplementedException();
    }

    protected virtual IJsBridgeResult? GenAppAuthKey(JsParam param)
    {
        throw new NotImplementedException();
    }

    protected virtual IJsBridgeResult? OpenSystemBrowser(JsParam param)
    {
        throw new NotImplementedException();
    }

    protected virtual IJsBridgeResult? SaveLoginTicket(JsParam param)
    {
        throw new NotImplementedException();
    }

    protected virtual ValueTask<IJsBridgeResult?> GetNotificationSettingsAsync(JsParam param)
    {
        throw new NotImplementedException();
    }

    protected virtual IJsBridgeResult? ShowToast(JsParam param)
    {
        throw new NotImplementedException();
    }

    protected virtual void DOMContentLoaded(CoreWebView2 coreWebView2)
    {
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

        logger?.LogInformation("[{Id}][ExecuteScript: {callback}]\n{payload}", bridgeId, callback, payload);

        await taskContext.SwitchToMainThreadAsync();

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (coreWebView2 is null || coreWebView2.IsDisposed())
        {
            return string.Empty;
        }

        return await coreWebView2.ExecuteScriptAsync(js);
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnWebMessageReceived(CoreWebView2 webView2, CoreWebView2WebMessageReceivedEventArgs args)
    {
        string message = args.TryGetWebMessageAsString();
        logger.LogInformation("[{Id}][OnRawMessage]\n{message}", bridgeId, message);
        JsParam? param = JsonSerializer.Deserialize<JsParam>(message);

        ArgumentNullException.ThrowIfNull(param);
        logger.LogInformation("[OnMessage]\nMethod  : {method}\nPayload : {payload}\nCallback: {callback}", param.Method, param.Payload, param.Callback);
        using (await webMessageLock.LockAsync().ConfigureAwait(false))
        {
            IJsBridgeResult? result = await TryGetJsResultFromJsParamAsync(param).ConfigureAwait(false);

            if (result is not null && param.Callback is not null)
            {
                await ExecuteCallbackScriptAsync(param.Callback, result.ToJson()).ConfigureAwait(false);
            }
        }
    }

    [SuppressMessage("", "CA2254")]
    private IJsBridgeResult? LogUnhandledMessage(string message, params object?[] param)
    {
        logger.LogWarning(message, param);
        return default;
    }

    private async ValueTask<IJsBridgeResult?> TryGetJsResultFromJsParamAsync(JsParam param)
    {
        if (coreWebView2.IsDisposed())
        {
            return default;
        }

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
                "getDS" => GetDataSignV1(param),
                "getDS2" => GetDataSignV2(param),
                "getHTTPRequestHeaders" => GetHttpRequestHeader(param),
                "getStatusBarHeight" => GetStatusBarHeight(param),
                "getUserInfo" => await GetUserInfoAsync(param).ConfigureAwait(false),
                "hideLoading" => null,
                "login" => null,
                "pushPage" => await PushPageAsync(param).ConfigureAwait(false),
                "share" => await ShareAsync(param).ConfigureAwait(false),
                "showLoading" => null,
                _ => LogUnhandledMessage("Unhandled Message Type: {Method}", param.Method),
            };
        }
        catch (InvalidOperationException)
        {
            // TODO: handle Json exception
            return default;
        }
    }

    private void OnDOMContentLoaded(CoreWebView2 coreWebView2, CoreWebView2DOMContentLoadedEventArgs args)
    {
        DOMContentLoaded(coreWebView2);
        coreWebView2.ExecuteScriptAsync(HideScrollBarScript).AsTask().SafeForget(logger);
        coreWebView2.ExecuteScriptAsync(ConvertMouseToTouchScript).AsTask().SafeForget(logger);
    }

    private void OnNavigationStarting(CoreWebView2 coreWebView2, CoreWebView2NavigationStartingEventArgs args)
    {
        string uriHost = new Uri(args.Uri).Host;
        ReadOnlySpan<char> uriHostSpan = uriHost.AsSpan();
        if (uriHostSpan.EndsWith("mihoyo.com") || uriHostSpan.EndsWith("hoyolab.com"))
        {
            coreWebView2.ExecuteScriptAsync(InitializeJsInterfaceScript).AsTask().SafeForget(logger);
        }
    }
}