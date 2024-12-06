// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage.Streams;

namespace Snap.Hutao.Core.DataTransfer;

internal interface IClipboardProvider
{
    ValueTask<T?> DeserializeFromJsonAsync<T>()
        where T : class;

    ValueTask<bool> SetBitmapAsync(IRandomAccessStream stream);

    ValueTask<bool> SetTextAsync(string text);
}