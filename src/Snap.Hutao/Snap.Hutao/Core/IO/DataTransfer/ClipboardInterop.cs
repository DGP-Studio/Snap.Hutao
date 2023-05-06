// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace Snap.Hutao.Core.IO.DataTransfer;

/// <summary>
/// 剪贴板互操作
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IClipboardInterop))]
internal sealed partial class ClipboardInterop : IClipboardInterop
{
    private readonly JsonSerializerOptions options;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async Task<T?> DeserializeFromJsonAsync<T>()
        where T : class
    {
        await taskContext.SwitchToMainThreadAsync();
        DataPackageView view = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

        if (view.Contains(StandardDataFormats.Text))
        {
            string json = await view.GetTextAsync();

            await taskContext.SwitchToBackgroundAsync();
            return JsonSerializer.Deserialize<T>(json, options);
        }

        return null;
    }

    /// <inheritdoc/>
    public bool SetText(string text)
    {
        try
        {
            DataPackage content = new() { RequestedOperation = DataPackageOperation.Copy };
            content.SetText(text);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(content);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public bool SetBitmap(IRandomAccessStream stream)
    {
        try
        {
            RandomAccessStreamReference reference = RandomAccessStreamReference.CreateFromStream(stream);
            DataPackage content = new() { RequestedOperation = DataPackageOperation.Copy };
            content.SetBitmap(reference);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(content);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
            return true;
        }
        catch
        {
            return false;
        }
    }
}