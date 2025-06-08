// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.Logging;
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
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.Web.Bridge;

internal class MiHoYoJSBridge
{
    private readonly Guid bridgeId = Guid.NewGuid();
    private readonly AsyncLock webMessageLock = new();
    private readonly UserAndUid userAndUid;

    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<MiHoYoJSBridge> logger;
    private readonly ITaskContext taskContext;

    private CoreWebView2? coreWebView2;

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

    public void Detach()
    {
        if (Interlocked.Exchange(ref this.coreWebView2, null) is { } coreWebView2)
        {
            coreWebView2.WebMessageReceived -= OnWebMessageReceived;
            coreWebView2.DOMContentLoaded -= OnDOMContentLoaded;
            coreWebView2.NavigationStarting -= OnNavigationStarting;
        }
    }

    protected virtual async ValueTask<IJsBridgeResult> ClosePageAsync(JsParam param)
    {
        await taskContext.SwitchToMainThreadAsync();
        if (coreWebView2 is { CanGoBack: true })
        {
            coreWebView2.GoBack();
        }
        else
        {
            ClosePageRequested?.Invoke();
        }

        return default!;
    }

    protected virtual ValueTask<IJsBridgeResult> ConfigureShareAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> EventTrackAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> GenAppAuthKeyAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> GenAuthKeyAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual async ValueTask<IJsBridgeResult> GetActionTicketAsync(JsParam<ActionTypePayload> jsParam)
    {
        return await serviceProvider
            .GetRequiredService<AuthClient>()
            .GetActionTicketBySTokenAsync(jsParam.Payload.ActionType, userAndUid.User)
            .ConfigureAwait(false);
    }

    protected virtual ValueTask<IJsBridgeResult> GetCookieInfoAsync(JsParam param)
    {
        ArgumentNullException.ThrowIfNull(userAndUid.User.LToken);

        IJsBridgeResult result = new JsResult<Dictionary<string, string>>()
        {
            Data = new()
            {
                [Cookie.LTUID] = userAndUid.User.LToken[Cookie.LTUID],
                [Cookie.LTOKEN] = userAndUid.User.LToken[Cookie.LTOKEN],
                [Cookie.LOGIN_TICKET] = string.Empty,
            },
        };

        return ValueTask.FromResult(result);
    }

    protected virtual async ValueTask<IJsBridgeResult> GetCookieTokenAsync(JsParam<CookieTokenPayload> param)
    {
        IUserService userService = serviceProvider.GetRequiredService<IUserService>();
        if (param.Payload.ForceRefresh)
        {
            await userService.RefreshCookieTokenAsync(userAndUid.User).ConfigureAwait(false);
        }

        await taskContext.SwitchToMainThreadAsync();
        coreWebView2.SetCookie(userAndUid.User.CookieToken, userAndUid.User.LToken);

        ArgumentNullException.ThrowIfNull(userAndUid.User.CookieToken);
        return new JsResult<Dictionary<string, string>>()
        {
            Data = new()
            {
                [Cookie.COOKIE_TOKEN] = userAndUid.User.CookieToken[Cookie.COOKIE_TOKEN]
            }
        };
    }

    protected virtual ValueTask<IJsBridgeResult> GetCurrentLocaleAsync(JsParam<PushPagePayload> param)
    {
        CultureOptions cultureOptions = serviceProvider.GetRequiredService<CultureOptions>();
        TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);

        IJsBridgeResult result = new JsResult<Dictionary<string, string>>()
        {
            Data = new()
            {
                ["language"] = cultureOptions.LanguageCode,
                ["timeZone"] = $"GMT{(offset.Hours >= 0 ? "+" : " - ")}{Math.Abs(offset.Hours):D1}",
            },
        };

        return ValueTask.FromResult(result);
    }

    protected virtual ValueTask<IJsBridgeResult> GetDataSignV1Async(JsParam param)
    {
        DataSignOptions options = DataSignOptions.CreateForGeneration1(SaltType.LK2, true);
        IJsBridgeResult result = new JsResult<Dictionary<string, string>>()
        {
            Data = new()
            {
                ["DS"] = DataSignAlgorithm.GetDataSign(options),
            },
        };

        return ValueTask.FromResult(result);
    }

    protected virtual ValueTask<IJsBridgeResult> GetDataSignV2Async(JsParam<DataSignV2Payload> param)
    {
        DataSignOptions options = DataSignOptions.CreateForGeneration2(SaltType.X4, false, param.Payload.Body, param.Payload.GetQueryParam());
        IJsBridgeResult result = new JsResult<Dictionary<string, string>>()
        {
            Data = new()
            {
                ["DS"] = DataSignAlgorithm.GetDataSign(options),
            },
        };

        return ValueTask.FromResult(result);
    }

    protected virtual ValueTask<IJsBridgeResult> GetHttpRequestHeaderAsync(JsParam param)
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

        GetHttpRequestHeaderOverride(headers);

        IJsBridgeResult result = new JsResult<Dictionary<string, string>>()
        {
            Data = headers,
        };

        return ValueTask.FromResult(result);
    }

    protected virtual void GetHttpRequestHeaderOverride(Dictionary<string, string> headers)
    {
    }

    protected virtual ValueTask<IJsBridgeResult> GetNotificationSettingsAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> GetStatusBarHeightAsync(JsParam param)
    {
        IJsBridgeResult result = new JsResult<Dictionary<string, object>>()
        {
            Data = new()
            {
                ["statusBarHeight"] = 0,
            },
        };
        return ValueTask.FromResult(result);
    }

    protected virtual async ValueTask<IJsBridgeResult> GetUserInfoAsync(JsParam param)
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
                return new JsResult<Dictionary<string, object>>();
            }
        }

        UserInfo info = wrapper.UserInfo;
        return new JsResult<Dictionary<string, object>>()
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

    protected virtual ValueTask<IJsBridgeResult> HideLoadingAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> IsFloatingWindowAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> LoginAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> OpenSystemBrowserAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual async ValueTask<IJsBridgeResult> PushPageAsync(JsParam<PushPagePayload> param)
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
        coreWebView2?.Navigate(targetUrl);
        return default!;
    }

    protected virtual ValueTask<IJsBridgeResult> SaveLoginTicketAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual async ValueTask<IJsBridgeResult> ShareAsync(JsParam<SharePayload> param)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            JsonSerializerOptions jsonSerializerOptions = scope.ServiceProvider.GetRequiredService<JsonSerializerOptions>();
            HttpClient httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            IClipboardProvider clipboardProvider = scope.ServiceProvider.GetRequiredService<IClipboardProvider>();
            IInfoBarService infoBarService = scope.ServiceProvider.GetRequiredService<IInfoBarService>();
            IFileSystemPickerInteraction fileSystemPickerInteraction = scope.ServiceProvider.GetRequiredService<IFileSystemPickerInteraction>();
            BridgeShareSaveType shareSaveType = scope.ServiceProvider.GetRequiredService<AppOptions>().BridgeShareSaveType;

            if (coreWebView2 is null)
            {
                return default!;
            }

            BridgeShareContext context = new(coreWebView2, taskContext, httpClient, infoBarService, clipboardProvider, jsonSerializerOptions, fileSystemPickerInteraction, shareSaveType);
            return await BridgeShare.ShareAsync(param, context).ConfigureAwait(false);
        }
    }

    protected virtual ValueTask<IJsBridgeResult> ShowAlertDialogAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> ShowLoadingAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> ShowToastAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> StartRealPersonValidationAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual ValueTask<IJsBridgeResult> StartRealnameAuthAsync(JsParam param)
    {
        return ValueTask.FromResult<IJsBridgeResult>(default!);
    }

    protected virtual void DOMContentLoaded(CoreWebView2 coreWebView2)
    {
    }

    private static void OnNavigationStarting(CoreWebView2 coreWebView2, CoreWebView2NavigationStartingEventArgs args)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Navigate to uri", "WebView2 MiHoYoJSBridge", [("Uri", args.Uri)]));
        string uriHost = args.Uri.ToUri().Host;
        ReadOnlySpan<char> uriHostSpan = uriHost.AsSpan();
        if (uriHostSpan.EndsWith("mihoyo.com") || uriHostSpan.EndsWith("hoyolab.com"))
        {
            coreWebView2.ExecuteScriptAsync(MiHoYoJavaScripts.InitializeJsInterfaceScript).AsTask().SafeForget();
        }
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

        logger.LogInformation("[{Id}][ExecuteScript: {callback}]\n{payload}", bridgeId, callback, payload);

        await taskContext.SwitchToMainThreadAsync();

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (coreWebView2 is null || coreWebView2.IsDisposed())
        {
            return string.Empty;
        }

        try
        {
            return await coreWebView2.ExecuteScriptAsync(js);
        }
        catch (COMException ex)
        {
            if (ex.HResult == unchecked((int)0x8007139F))
            {
                // 组或资源的状态不是执行请求操作的正确状态。
                return string.Empty;
            }

            throw;
        }
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnWebMessageReceived(CoreWebView2 webView2, CoreWebView2WebMessageReceivedEventArgs args)
    {
        string message = args.TryGetWebMessageAsString();
        logger.LogInformation("[{Id}][OnRawMessage]\n{message}", bridgeId, message);
        JsParam? param = JsonSerializer.Deserialize<JsParam>(message);

        ArgumentNullException.ThrowIfNull(param);
        logger.LogInformation("[OnMessage]\nMethod  : {method}\nPayload : {payload}\nCallback: {callback}", param.Method, param.Payload, param.Callback);

        try
        {
            using (await webMessageLock.LockAsync().ConfigureAwait(false))
            {
                IJsBridgeResult? result = await TryGetJsResultFromJsParamAsync(param).ConfigureAwait(false);

                if (result is not null && param.Callback is not null)
                {
                    await ExecuteCallbackScriptAsync(param.Callback, JsonSerializer.Serialize(result, result.GetType())).ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
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
                "configure_share" => await ConfigureShareAsync(param).ConfigureAwait(false),
                "eventTrack" => await EventTrackAsync(param).ConfigureAwait(false),
                "getActionTicket" => await GetActionTicketAsync(param).ConfigureAwait(false),
                "getCookieInfo" => await GetCookieInfoAsync(param).ConfigureAwait(false),
                "getCookieToken" => await GetCookieTokenAsync(param).ConfigureAwait(false),
                "getCurrentLocale" => await GetCurrentLocaleAsync(param).ConfigureAwait(false),
                "getDS" => await GetDataSignV1Async(param).ConfigureAwait(false),
                "getDS2" => await GetDataSignV2Async(param).ConfigureAwait(false),
                "getHTTPRequestHeaders" => await GetHttpRequestHeaderAsync(param).ConfigureAwait(false),
                "getStatusBarHeight" => await GetStatusBarHeightAsync(param).ConfigureAwait(false),
                "getUserInfo" => await GetUserInfoAsync(param).ConfigureAwait(false),
                "hideLoading" => await HideLoadingAsync(param).ConfigureAwait(false),
                "isFloatingWindow" => await IsFloatingWindowAsync(param).ConfigureAwait(false),
                "login" => await LoginAsync(param).ConfigureAwait(false),
                "pushPage" => await PushPageAsync(param).ConfigureAwait(false),
                "share" => await ShareAsync(param).ConfigureAwait(false),
                "showLoading" => await ShowLoadingAsync(param).ConfigureAwait(false),
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
        coreWebView2.ExecuteScriptAsync(MiHoYoJavaScripts.HideScrollBarScript).AsTask().SafeForget();
        coreWebView2.ExecuteScriptAsync(MiHoYoJavaScripts.ConvertMouseToTouchScript).AsTask().SafeForget();
    }
}