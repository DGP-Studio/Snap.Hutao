// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Service.Notification;
using System.Net.Http;

namespace Snap.Hutao.Web.Bridge;

internal sealed class BridgeShareContext
{
    private readonly CoreWebView2 coreWebView2;
    private readonly ITaskContext taskContext;
    private readonly HttpClient httpClient;
    private readonly IInfoBarService infoBarService;
    private readonly IClipboardProvider clipboardProvider;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public BridgeShareContext(CoreWebView2 coreWebView2, ITaskContext taskContext, HttpClient httpClient, IInfoBarService infoBarService, IClipboardProvider clipboardProvider, JsonSerializerOptions jsonSerializerOptions)
    {
        this.httpClient = httpClient;
        this.taskContext = taskContext;
        this.infoBarService = infoBarService;
        this.clipboardProvider = clipboardProvider;
        this.coreWebView2 = coreWebView2;
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    public CoreWebView2 CoreWebView2 { get => coreWebView2; }

    public ITaskContext TaskContext { get => taskContext; }

    public HttpClient HttpClient { get => httpClient; }

    public IInfoBarService InfoBarService { get => infoBarService; }

    public IClipboardProvider ClipboardProvider { get => clipboardProvider; }

    public JsonSerializerOptions JsonSerializerOptions { get => jsonSerializerOptions; }
}
