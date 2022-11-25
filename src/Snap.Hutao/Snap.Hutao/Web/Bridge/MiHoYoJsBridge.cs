// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Web.Bridge.Model;
using Snap.Hutao.Web.Bridge.Model.Event;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// 调用桥
/// </summary>
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public sealed class MiHoYoJsBridge : IMiHoYoJsBridge
{
    private readonly CoreWebView2 webView;
    private readonly ILogger<MiHoYoJsBridge>? logger;

    private readonly Dictionary<string, string> jsWebInvokeTypeCache = new();
    private readonly Dictionary<string, Action<JsParam>> callbackHandlers = new();

    /// <summary>
    /// 构造一个新的调用桥
    /// </summary>
    /// <param name="webView">webview2</param>
    /// <param name="logger">日志器</param>
    internal MiHoYoJsBridge(CoreWebView2 webView, ILogger<MiHoYoJsBridge>? logger = null)
    {
        this.webView = webView;
        this.logger = logger;
    }

    /// <summary>
    /// 消息发生时调用
    /// </summary>
    /// <param name="message">消息</param>
    public void OnMessage(string message)
    {
        logger?.LogInformation("[OnMessage] {message}", message);

        JsParam p = JsonSerializer.Deserialize<JsParam>(message)!;
        p.Bridge = this;

        callbackHandlers.GetValueOrDefault(p.Method)?.Invoke(p);
    }

    /// <summary>
    /// 调用JS回调
    /// </summary>
    /// <param name="callbackName">回调名称</param>
    /// <param name="payload">传输的数据</param>
    /// <returns>执行结果</returns>
    public Task<string> InvokeJsCallbackAsync(string callbackName, string? payload = null)
    {
        if (string.IsNullOrEmpty(callbackName))
        {
            return Task.FromResult(string.Empty);
        }

        string dataStr = payload == null ? string.Empty : $", {payload}";
        string js = $"javascript:mhyWebBridge(\"{callbackName}\"{dataStr})";
        logger?.LogInformation("[InvokeJsCallback] {js}", js);
        return webView.ExecuteScriptAsync(js).AsTask();
    }

    /// <summary>
    /// 注册回调
    /// </summary>
    /// <typeparam name="T">回调类型</typeparam>
    /// <param name="callback">回调</param>
    /// <returns>桥</returns>
    public MiHoYoJsBridge Register<T>(Action<JsParam> callback)
        where T : notnull
    {
        callbackHandlers[GetCallbackName<T>()] = callback;
        return this;
    }

    /// <summary>
    /// 注册回调
    /// </summary>
    /// <typeparam name="T">回调类型</typeparam>
    /// <param name="callback">回调</param>
    /// <returns>桥</returns>
    public MiHoYoJsBridge Register<T>(Action<JsParam, T> callback)
        where T : notnull
    {
        callbackHandlers[GetCallbackName<T>()] = p => callback(p, p.Data.As<T>());
        return this;
    }

    private string GetCallbackName<T>()
    {
        Type type = typeof(T);
        string invokeName = type.GetCustomAttribute<WebInvokeAttribute>()?.Name
            ?? throw new ArgumentException("Type Callback not registered.");

        return jsWebInvokeTypeCache[type.Name] = invokeName;
    }
}