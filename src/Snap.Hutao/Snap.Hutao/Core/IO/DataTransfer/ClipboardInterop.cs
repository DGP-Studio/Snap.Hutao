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
    public async ValueTask<T?> DeserializeFromJsonAsync<T>()
        where T : class
    {
        await taskContext.SwitchToMainThreadAsync();
        DataPackageView view = Clipboard.GetContent();

        if (!view.Contains(StandardDataFormats.Text))
        {
            return null;
        }

        string json = await view.GetTextAsync();

        await taskContext.SwitchToBackgroundAsync();
        return JsonSerializer.Deserialize<T>(json, options);
    }

    /// <inheritdoc/>
    public bool SetText(string text)
    {
        try
        {
            DataPackage content = new() { RequestedOperation = DataPackageOperation.Copy };
            content.SetText(text);
            Clipboard.SetContent(content);
            Clipboard.Flush();
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
            Clipboard.SetContent(content);
            Clipboard.Flush();
            return true;
        }
        catch
        {
            return false;
        }
    }
}