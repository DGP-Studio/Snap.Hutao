// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage.Streams;

namespace Snap.Hutao.Core.DataTransfer;

internal interface IClipboardProvider
{
    ValueTask<T?> DeserializeFromJsonAsync<T>()
        where T : class;

    bool SetBitmap(IRandomAccessStream stream);

    ValueTask<bool> SetBitmapAsync(IRandomAccessStream stream);

    bool SetText(string text);

    ValueTask<bool> SetTextAsync(string text);
}