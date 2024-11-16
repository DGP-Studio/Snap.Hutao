// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Notification;
using System.Net.Http;

namespace Snap.Hutao.Web.Bridge;

internal sealed class BridgeShareContext
{
    public BridgeShareContext(CoreWebView2 coreWebView2, ITaskContext taskContext, HttpClient httpClient, IInfoBarService infoBarService, IClipboardProvider clipboardProvider, JsonSerializerOptions jsonSerializerOptions, IFileSystemPickerInteraction fileSystemPickerInteraction, BridgeShareSaveType shareSaveType)
    {
        HttpClient = httpClient;
        TaskContext = taskContext;
        InfoBarService = infoBarService;
        ClipboardProvider = clipboardProvider;
        CoreWebView2 = coreWebView2;
        JsonSerializerOptions = jsonSerializerOptions;
        FileSystemPickerInteraction = fileSystemPickerInteraction;
        ShareSaveType = shareSaveType;
    }

    public CoreWebView2 CoreWebView2 { get; }

    public ITaskContext TaskContext { get; }

    public HttpClient HttpClient { get; }

    public IInfoBarService InfoBarService { get; }

    public IClipboardProvider ClipboardProvider { get; }

    public JsonSerializerOptions JsonSerializerOptions { get; }

    public IFileSystemPickerInteraction FileSystemPickerInteraction { get; }

    public BridgeShareSaveType ShareSaveType { get; }
}
