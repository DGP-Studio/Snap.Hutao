// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Core;

internal static class Uuid
{
    public static Guid NewV5(string name, Guid namespaceId)
    {
        Span<byte> namespaceBuffer = stackalloc byte[16];
        Verify.Operation(namespaceId.TryWriteBytes(namespaceBuffer), "Failed to copy namespace guid bytes");
        Span<byte> nameBytes = Encoding.UTF8.GetBytes(name);

        if (BitConverter.IsLittleEndian)
        {
            ReverseEndianness(namespaceBuffer);
        }

        Span<byte> data = stackalloc byte[namespaceBuffer.Length + nameBytes.Length];
        namespaceBuffer.CopyTo(data);
        nameBytes.CopyTo(data[namespaceBuffer.Length..]);

        Span<byte> temp = stackalloc byte[20];
        Verify.Operation(SHA1.TryHashData(data, temp, out _), "Failed to compute SHA1 hash of UUID");

        Span<byte> hash = temp[..16];

        if (BitConverter.IsLittleEndian)
        {
            ReverseEndianness(hash);
        }

        hash[8] &= 0x3F;
        hash[8] |= 0x80;

        int versionIndex = BitConverter.IsLittleEndian ? 7 : 6;

        hash[versionIndex] &= 0x0F;
        hash[versionIndex] |= 0x50;

        return new(hash);
    }

    private static void ReverseEndianness(in Span<byte> guidByte)
    {
        ExchangeBytes(guidByte, 0, 3);
        ExchangeBytes(guidByte, 1, 2);
        ExchangeBytes(guidByte, 4, 5);
        ExchangeBytes(guidByte, 6, 7);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ExchangeBytes(in Span<byte> guid, int left, int right)
    {
        (guid[right], guid[left]) = (guid[left], guid[right]);
    }
}