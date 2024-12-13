// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

internal static class Random
{
    public static string GetLowerHexString(int length)
    {
        Span<char> buffer = stackalloc char[length];
        System.Random.Shared.GetItems("0123456789abcdef", buffer);
        return buffer.ToString();
    }

    public static void GetLowerHexString(Span<char> destination)
    {
        System.Random.Shared.GetItems("0123456789abcdef", destination);
    }

    public static string GetUpperAndNumberString(int length)
    {
        Span<char> buffer = stackalloc char[length];
        System.Random.Shared.GetItems("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ", buffer);
        return buffer.ToString();
    }

    public static void GetUpperAndNumberString(Span<char> destination)
    {
        System.Random.Shared.GetItems("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ", destination);
    }

    public static string GetLowerAndNumberString(int length)
    {
        Span<char> buffer = stackalloc char[length];
        System.Random.Shared.GetItems("0123456789abcdefghijklmnopqrstuvwxyz", buffer);
        return buffer.ToString();
    }

    public static void GetLowerAndNumberString(Span<char> destination)
    {
        System.Random.Shared.GetItems("0123456789abcdefghijklmnopqrstuvwxyz", destination);
    }
}