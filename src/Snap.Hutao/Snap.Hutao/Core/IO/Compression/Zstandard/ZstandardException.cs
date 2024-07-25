// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Text;
using static Snap.ZStandard.Methods;

namespace Snap.Hutao.Core.IO.Compression.Zstandard;

internal sealed class ZstandardException : Exception
{
    public ZstandardException(string message)
        : base(message)
    {
    }

    public unsafe ZstandardException(nuint code)
        : base(Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)ZSTD_getErrorName(code))))
    {
        ErrorCode = code;
    }

    public static unsafe void ThrowIfNull(void* ptr, string message)
    {
        if (ptr is null)
        {
            throw new ZstandardException(message);
        }
    }

    public static void ThrowIfError(nuint code)
    {
        if (ZSTD_isError(code) is not 0)
        {
            throw new ZstandardException(code);
        }
    }

    public nuint ErrorCode { get; set; }
}