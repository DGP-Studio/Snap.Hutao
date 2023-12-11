// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

internal static class Random
{
    public static string GetLowerHexString(int length)
    {
        return new(System.Random.Shared.GetItems("0123456789abcdef".AsSpan(), length));
    }

    public static string GetUpperAndNumberString(int length)
    {
        return new(System.Random.Shared.GetItems("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".AsSpan(), length));
    }

    public static string GetLowerAndNumberString(int length)
    {
        return new(System.Random.Shared.GetItems("0123456789abcdefghijklmnopqrstuvwxyz".AsSpan(), length));
    }

    public static string GetLetterAndNumberString(int length)
    {
        return new(System.Random.Shared.GetItems("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".AsSpan(), length));
    }
}