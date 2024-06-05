// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Core.IO.Hashing;

#if NET9_0_OR_GREATER
[Obsolete]
#endif
internal static class Hash
{
    public static unsafe string SHA1HexString(string input)
    {
        return HashCore(Convert.ToHexString, SHA1.HashData, Encoding.UTF8.GetBytes, input);
    }

    public static unsafe string MD5HexString(string input)
    {
        return HashCore(Convert.ToHexString, System.Security.Cryptography.MD5.HashData, Encoding.UTF8.GetBytes, input);
    }

    private static unsafe TResult HashCore<TInput, TResult>(Func<byte[], TResult> resultConverter, Func<byte[], byte[]> hashMethod, Func<TInput, byte[]> bytesConverter, TInput input)
    {
        return resultConverter(hashMethod(bytesConverter(input)));
    }
}
