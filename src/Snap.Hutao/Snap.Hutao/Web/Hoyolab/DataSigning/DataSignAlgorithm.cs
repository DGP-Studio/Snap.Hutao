// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Hashing;
using System.Security.Cryptography;

namespace Snap.Hutao.Web.Hoyolab.DataSigning;

internal static class DataSignAlgorithm
{
    public static string GetDataSign(DataSignOptions options)
    {
        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string r = options.RandomString;

        string dsContent = $"salt={options.Salt}&t={t}&r={options.RandomString}";

        // ds2 b & q process
        if (options.RequiresBodyAndQuery)
        {
            dsContent = $"{dsContent}&b={options.Body}&q={options.Query}";
        }

#pragma warning disable CA1308
        string check = Hash.ToHexString(HashAlgorithmName.MD5, dsContent).ToLowerInvariant();
#pragma warning restore CA1308

        return $"{t},{r},{check}";
    }
}