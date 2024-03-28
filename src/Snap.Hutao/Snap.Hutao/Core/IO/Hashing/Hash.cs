// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Core.IO.Hashing;

internal static class Hash
{
    public static string SHA1HexString(string input)
    {
        return HashCore(BitConverter.ToString, SHA1.HashData, Encoding.UTF8.GetBytes, input);
    }

    private static TResult HashCore<TInput, TResult>(Func<byte[], TResult> resultConverter, Func<byte[], byte[]> hashMethod, Func<TInput, byte[]> bytesConverter, TInput input)
    {
        return resultConverter(hashMethod(bytesConverter(input)));
    }
}
