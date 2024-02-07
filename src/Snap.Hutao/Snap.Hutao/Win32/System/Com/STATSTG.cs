// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

// [StructSizeField("cbSize")]
[SuppressMessage("", "SA1307")]
internal struct STATSTG
{
    public PWSTR pwcsName;
    public uint type;
    public ulong cbSize;
    public FILETIME mtime;
    public FILETIME ctime;
    public FILETIME atime;
    public STGM grfMode;

    // [AssociatedEnum("LOCKTYPE")]
    public uint grfLocksSupported;
    public Guid clsid;
    public uint grfStateBits;
    public uint reserved;
}