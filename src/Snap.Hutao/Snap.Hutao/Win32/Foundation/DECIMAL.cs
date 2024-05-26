// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1300")]
[SuppressMessage("", "SA1307")]
internal struct DECIMAL
{
    public ushort wReserved;

    public byte scale;

    public byte sign;

    [SuppressMessage("", "IDE1006")]
    public unsafe ushort signscale
    {
        get
        {
            fixed (DECIMAL* pThis = &this)
            {
                return *(ushort*)&pThis->scale;
            }
        }

        set
        {
            fixed (DECIMAL* pThis = &this)
            {
                *(ushort*)&pThis->scale = value;
            }
        }
    }

    public uint Hi32;

    public uint Lo32;

    public uint Mid32;

    public unsafe ulong Lo64
    {
        get
        {
            fixed (DECIMAL* pThis = &this)
            {
                return *(ulong*)&pThis->Lo32;
            }
        }

        set
        {
            fixed (DECIMAL* pThis = &this)
            {
                *(ulong*)&pThis->Lo32 = value;
            }
        }
    }
}