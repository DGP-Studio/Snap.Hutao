// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO.Hashing;

internal static class SHA256
{
    public static async ValueTask<string> HashFileAsync(string filePath, CancellationToken token = default)
    {
        using (FileStream stream = File.OpenRead(filePath))
        {
            return await HashAsync(stream, token).ConfigureAwait(false);
        }
    }

    public static async ValueTask<string> HashAsync(Stream stream, CancellationToken token = default)
    {
        byte[] bytes = await System.Security.Cryptography.SHA256.HashDataAsync(stream, token).ConfigureAwait(false);
        return Convert.ToHexString(bytes);
    }
}