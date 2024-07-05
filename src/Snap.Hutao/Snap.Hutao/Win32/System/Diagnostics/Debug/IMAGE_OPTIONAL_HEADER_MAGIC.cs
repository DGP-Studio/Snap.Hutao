// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Diagnostics.Debug;

[SuppressMessage("", "CA1069")]
internal enum IMAGE_OPTIONAL_HEADER_MAGIC : ushort
{
    IMAGE_NT_OPTIONAL_HDR_MAGIC = 523,
    IMAGE_NT_OPTIONAL_HDR32_MAGIC = 267,
    IMAGE_NT_OPTIONAL_HDR64_MAGIC = 523,
    IMAGE_ROM_OPTIONAL_HDR_MAGIC = 263,
}