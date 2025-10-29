// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace Snap.Hutao.Core.DataTransfer;

[Service(ServiceLifetime.Transient, typeof(IClipboardProvider))]
internal sealed partial class DefaultClipboardSource : IClipboardProvider
{
    private readonly JsonSerializerOptions options;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial DefaultClipboardSource(IServiceProvider serviceProvider);

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

    public async ValueTask<bool> SetTextAsync(string text)
    {
        try
        {
            await taskContext.SwitchToMainThreadAsync();
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

    public async ValueTask<bool> SetBitmapAsync(IRandomAccessStream stream)
    {
        try
        {
            await taskContext.SwitchToMainThreadAsync();
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