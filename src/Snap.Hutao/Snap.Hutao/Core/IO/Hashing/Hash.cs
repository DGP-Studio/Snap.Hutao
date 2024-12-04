// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Core.IO.Hashing;

internal static class Hash
{
    public static string ToHexString(HashAlgorithmName hashAlgorithm, string input)
    {
        return Convert.ToHexString(CryptographicOperations.HashData(hashAlgorithm, Encoding.UTF8.GetBytes(input)));
    }

    public static string ToHexStringLower(HashAlgorithmName hashAlgorithm, string input)
    {
        return Convert.ToHexStringLower(CryptographicOperations.HashData(hashAlgorithm, Encoding.UTF8.GetBytes(input)));
    }

    public static string ToHexString(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> input)
    {
        return Convert.ToHexString(CryptographicOperations.HashData(hashAlgorithm, input));
    }

    public static async ValueTask<string> ToHexStringAsync(HashAlgorithmName hashAlgorithm, Stream input, CancellationToken token = default)
    {
        return Convert.ToHexString(await CryptographicOperations.HashDataAsync(hashAlgorithm, input, token).ConfigureAwait(false));
    }

    public static async ValueTask<string> FileToHexStringAsync(HashAlgorithmName hashAlgorithm, string filePath, CancellationToken token = default)
    {
        using (FileStream stream = File.OpenRead(filePath))
        {
            return await ToHexStringAsync(hashAlgorithm, stream, token).ConfigureAwait(false);
        }
    }
}