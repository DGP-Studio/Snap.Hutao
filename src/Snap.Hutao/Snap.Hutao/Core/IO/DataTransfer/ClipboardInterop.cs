// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage.Streams;

namespace Snap.Hutao.Core.IO.DataTransfer;

/// <summary>
/// 剪贴板互操作
/// </summary>
[Injection(InjectAs.Transient, typeof(IClipboardInterop))]
internal sealed class ClipboardInterop : IClipboardInterop
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的剪贴板互操作对象
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public ClipboardInterop(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public Task<T?> DeserializeTextAsync<T>()
        where T : class
    {
        return Clipboard.DeserializeTextAsync<T>(serviceProvider);
    }

    /// <inheritdoc/>
    public bool SetBitmap(IRandomAccessStream stream)
    {
        try
        {
            Clipboard.SetBitmap(stream);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}