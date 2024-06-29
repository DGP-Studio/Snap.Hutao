// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Bridge.Model;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Snap.Hutao.Web.Bridge;

internal sealed partial class BridgeShareImplmentation
{
    public static async ValueTask<IJsBridgeResult?> ShareAsync(JsParam<SharePayload> param, BridgeShareContext context)
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
                    string jsonParameters = """{ "format": "png", "captureBeyondViewport": true }""";
                    string resultJson = await context.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", jsonParameters);

                    CaptureScreenshotResult? result = JsonSerializer.Deserialize<CaptureScreenshotResult>(resultJson, context.JsonSerializerOptions);
                    ArgumentNullException.ThrowIfNull(result);

                    await ShareFromRawPixelDataAsync(context, result.Data).ConfigureAwait(false);
                    break;
                }
        }

        return new JsResult<Dictionary<string, string>>()
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
            await ShareCoreAsync(context, stream, static (stream, web) => web.CopyToAsync(stream.AsStreamForWrite())).ConfigureAwait(false);
        }
    }

    private static ValueTask ShareFromImageBase64Async(BridgeShareContext context, string base64ImageData)
    {
        return ShareFromRawPixelDataAsync(context, Convert.FromBase64String(base64ImageData));
    }

    private static ValueTask ShareFromRawPixelDataAsync(BridgeShareContext context, byte[] rawPixelData)
    {
        return ShareCoreAsync(context, rawPixelData, static (stream, bytes) => stream.AsStreamForWrite().WriteAsync(bytes).AsTask());
    }

    private static async ValueTask ShareCoreAsync<TData>(BridgeShareContext context, TData data, Func<InMemoryRandomAccessStream, TData, Task> asyncWriteData)
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

                if (await context.ClipboardProvider.SetBitmapAsync(stream).ConfigureAwait(false))
                {
                    context.InfoBarService.Success(SH.WebBridgeShareCopyToClipboardSuccess);
                }
                else
                {
                    context.InfoBarService.Error(SH.WebBridgeShareCopyToClipboardFailed);
                }
            }
        }
    }

    private sealed class CaptureScreenshotResult
    {
        [JsonPropertyName("data")]
        public byte[] Data { get; set; } = default!;
    }
}