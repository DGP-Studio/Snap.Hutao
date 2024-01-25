// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Registry;

[SuppressMessage("", "SA1310")]
internal readonly struct HKEY
{
    public static readonly HKEY HKEY_CLASSES_ROOT = unchecked((int)0x80000000);
    public static readonly HKEY HKEY_CURRENT_USER = unchecked((int)0x80000001);
    public static readonly HKEY HKEY_LOCAL_MACHINE = unchecked((int)0x80000002);
    public static readonly HKEY HKEY_USERS = unchecked((int)0x80000003);
    public static readonly HKEY HKEY_CURRENT_CONFIG = unchecked((int)0x80000005);

    public readonly nint Value;

    public static unsafe implicit operator HKEY(int value) => *(HKEY*)&value;
}