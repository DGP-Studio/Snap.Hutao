// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Diagnostics.Debug;

internal struct IMAGE_NT_HEADERS64
{
    public uint Signature;
    public IMAGE_FILE_HEADER FileHeader;
    public IMAGE_OPTIONAL_HEADER64 OptionalHeader;
}