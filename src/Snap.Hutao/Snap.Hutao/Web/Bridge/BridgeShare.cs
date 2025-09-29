// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Bridge.Model;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Snap.Hutao.Web.Bridge;

internal static class BridgeShare
{
    public static async ValueTask<IJsBridgeResult> ShareAsync(JsParam<SharePayload> param, BridgeShareContext context)
    {
        SharePayload payload = param.Payload;
        switch (payload.Type)
        {
            case "image":
                {
                    ShareContent content = payload.Content;
                    if (content.ImageUrl is { Length: > 0 } imageUrl)
                    {
                        await ShareFromImageUrlAsync(context, imageUrl).ConfigureAwait(false);
                    }
                    else if (content.ImageBase64 is { } imageBase64)
                    {
                        await ShareFromImageBase64Async(context, imageBase64).ConfigureAwait(false);
                    }

                    break;
                }

            case "screenshot":
                {
                    await context.TaskContext.SwitchToMainThreadAsync();

                    // https://chromedevtools.github.io/devtools-protocol/tot/Page/#method-captureScreenshot
                    const string JsonParameters = /*lang=json*/"""{ "format": "png", "captureBeyondViewport": true }""";
                    string resultJson = await context.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", JsonParameters);

                    CaptureScreenshotResult? result = JsonSerializer.Deserialize<CaptureScreenshotResult>(resultJson, context.JsonSerializerOptions);
                    ArgumentNullException.ThrowIfNull(result);

                    await ShareFromRawPixelDataAsync(context, result.Data).ConfigureAwait(false);
                    break;
                }
        }

        return new JsResult<Dictionary<string, string>>
        {
            Data = new()
            {
                ["type"] = param.Payload.Type,
            },
        };
    }

    private static async ValueTask ShareFromImageUrlAsync(BridgeShareContext context, string imageUrl)
    {
        using (Stream stream = await context.HttpClient.GetStreamAsync(imageUrl).ConfigureAwait(false))
        {
            await PrivateShareAsync(context, stream, static (stream, web) => web.CopyToAsync(stream.AsStreamForWrite())).ConfigureAwait(false);
        }
    }

    private static ValueTask ShareFromImageBase64Async(BridgeShareContext context, string base64ImageData)
    {
        byte[] data;
        try
        {
            data = Convert.FromBase64String(base64ImageData);
        }
        catch (Exception ex)
        {
            ExceptionAttachment.SetAttachment(ex, "Image_Base64.txt", base64ImageData);
            throw;
        }

        return ShareFromRawPixelDataAsync(context, data);
    }

    private static ValueTask ShareFromRawPixelDataAsync(BridgeShareContext context, byte[] rawPixelData)
    {
        return PrivateShareAsync(context, rawPixelData, static (stream, bytes) => stream.AsStreamForWrite().WriteAsync(bytes).AsTask());
    }

    private static async ValueTask PrivateShareAsync<TData>(BridgeShareContext context, TData data, Func<InMemoryRandomAccessStream, TData, Task> asyncWriteData)
    {
        using (InMemoryRandomAccessStream rawPixelDataStream = new())
        {
            await asyncWriteData(rawPixelDataStream, data).ConfigureAwait(false);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(rawPixelDataStream);

            using (InMemoryRandomAccessStream stream = new())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetSoftwareBitmap(await decoder.GetSoftwareBitmapAsync());
                await encoder.FlushAsync();

                ValueTask task = context.ShareSaveType switch
                {
                    BridgeShareSaveType.CopyToClipboard => CopyToClipboardAsync(context, stream),
                    BridgeShareSaveType.SaveAsFile => SaveAsFileAsync(context, stream),
                    _ => ValueTask.CompletedTask,
                };

                await task.ConfigureAwait(false);
            }
        }
    }

    private static async ValueTask CopyToClipboardAsync(BridgeShareContext context, InMemoryRandomAccessStream stream)
    {
        _ = await context.ClipboardProvider.SetBitmapAsync(stream).ConfigureAwait(false)
            ? context.Messenger.Send(InfoBarMessage.Success(SH.WebBridgeShareCopyToClipboardSuccess))
            : context.Messenger.Send(InfoBarMessage.Error(SH.WebBridgeShareCopyToClipboardFailed));
    }

    private static async ValueTask SaveAsFileAsync(BridgeShareContext context, InMemoryRandomAccessStream stream)
    {
        (bool isOk, ValueFile file) = context.FileSystemPickerInteraction.SaveFile(SH.WebBridgeShareFilePickerTitle, "share.png", "PNG", "*.png");
        if (!isOk)
        {
            return;
        }

        try
        {
            using (FileStream fileStream = File.Create(file))
            {
                stream.Seek(0);
                await stream.AsStreamForRead().CopyToAsync(fileStream).ConfigureAwait(false);
            }

            context.Messenger.Send(InfoBarMessage.Success(SH.WebBridgeShareSaveAsFileSuccess));
        }
        catch
        {
            context.Messenger.Send(InfoBarMessage.Error(SH.WebBridgeShareSaveAsFileFailed));
        }
    }

    private sealed class CaptureScreenshotResult
    {
        [JsonPropertyName("data")]
        public byte[] Data { get; set; } = default!;
    }
}