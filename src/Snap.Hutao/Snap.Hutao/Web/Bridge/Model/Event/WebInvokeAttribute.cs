// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model.Event;

/// <summary>
/// Web 调用
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class WebInvokeAttribute : Attribute
{
    /// <summary>
    /// 构造一个新的Web 调用特性
    /// </summary>
    /// <param name="name">函数名称</param>
    public WebInvokeAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 调用函数名称
    /// </summary>
    public string Name { get; init; }
}

public class ButtonParam
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("style")]
    public string Style { get; set; } = default!;
}

public abstract class GenAuthKeyBase
{
    [JsonPropertyName("game_biz")]
    public string Biz { get; set; } = default!;

    [JsonPropertyName("auth_appid")]
    public string AppId { get; set; } = default!;

    [JsonPropertyName("game_uid")]
    public uint Uid { get; set; }

    [JsonPropertyName("region")]
    public string Region { get; set; } = default!;
}

[WebInvoke("closePage")]
public struct JsEventClosePage
{
}

[WebInvoke("configure_share")]
public class JsEventConfigureShare
{
    [JsonPropertyName("enable")]
    public bool Enable { get; set; }
}

[WebInvoke("genAppAuthKey")]
public class JsEventGenAppAuthKey
    : GenAuthKeyBase
{
}

[WebInvoke("genAuthKey")]
public class JsEventGenAuthKey
    : GenAuthKeyBase
{
}

[WebInvoke("getActionTicket")]
public class JsEventGetActionTicket
{
    [JsonPropertyName("action_type")]
    public string ActionType { get; set; } = default!;
}

[WebInvoke("getCookieToken")]
public class JsEventGetCookieToken
{
    [JsonPropertyName("forceRefresh")]
    public bool ForceRefresh { get; set; }
}

[WebInvoke("getDS")]
public struct JsEventGetDynamicSecretV1
{
}

[WebInvoke("getDS2")]
public class JsEventGetDynamicSecretV2
{
    [JsonPropertyName("query")]
    public Dictionary<string, string> Query { get; set; } = new();

    [JsonPropertyName("body")]
    public string Body { get; set; } = default!;
}

[WebInvoke("getNotificationSettings")]
public struct JsEventGetNotificationSettings
{
}

[WebInvoke("startRealnameAuth")]
public struct JsEventGetRealNameStatus
{
    // guess
}

[WebInvoke("getHTTPRequestHeaders")]
public struct JsEventGetRequestHeader
{
}

[WebInvoke("getStatusBarHeight")]
public struct JsEventGetStatusBarHeight
{
    // just zero
}

[WebInvoke("getUserInfo")]
public struct JsEventGetUserInfo
{
}

[WebInvoke("getCookieInfo")]
public struct JsEventGetWebLoginInfo
{
}

[WebInvoke("openSystemBrowser")]
public class JsEventOpenSystemBrowser
{
    [JsonPropertyName("open_url")]
    public string PageUrl { get; set; } = default!;
}

[WebInvoke("pushPage")]
public class JsEventPushPage
{
    private string pageUrl = default!;

    [JsonPropertyName("page")]
    public string PageUrl
    {
        get => pageUrl;
        set => SetPageUrl(value);
    }

    private void SetPageUrl(string value)
    {
        pageUrl = value.StartsWith("mihoyobbs")
           ? value.Replace("mihoyobbs://", "https://bbs.mihoyo.com/dby/").Replace("topic", "topicDetail")
           : value;
    }
}

[WebInvoke("startRealPersonValidation")]
public struct JsEventRealPersonValidation
{
}

[WebInvoke("saveLoginTicket")]
public class JsEventSaveLoginTicket
{
    [JsonPropertyName("login_ticket")]
    public string LoginTicket { get; set; } = default!;
}

[WebInvoke("showAlertDialog")]
public class JsEventShowAlertDialog
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

    [JsonPropertyName("buttons")]
    public List<ButtonParam> Buttons { get; set; } = new();
}

[WebInvoke("showToast")]
public class JsEventShowToast
{
    [JsonPropertyName("toast")]
    public string Text { get; set; } = default!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;
}