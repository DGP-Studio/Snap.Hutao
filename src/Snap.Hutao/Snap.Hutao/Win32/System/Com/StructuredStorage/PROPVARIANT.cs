// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Variant;

namespace Snap.Hutao.Win32.System.Com.StructuredStorage;

[SuppressMessage("", "IDE1006")]
[SuppressMessage("", "SA1300")]
[SuppressMessage("", "SA1307")]
internal struct PROPVARIANT
{
    public VARENUM vt;
    public ushort wReserved1;
    public ushort wReserved2;
    public ushort wReserved3;
    public nint Value;

    // https://learn.microsoft.com/zh-cn/windows/win32/api/propidlbase/ns-propidlbase-propvariant
    public unsafe DECIMAL decVal
    {
        get
        {
            fixed (PROPVARIANT* pThis = &this)
            {
                return *(DECIMAL*)pThis;
            }
        }

        set
        {
            fixed (PROPVARIANT* pThis = &this)
            {
                *(DECIMAL*)pThis = value;
            }
        }
    }
}
