// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

[SuppressMessage("", "SA1307")]
internal struct INPUT
{
    public INPUT_TYPE type;
    public AnonymousUnion Anonymous;

    [StructLayout(LayoutKind.Explicit)]
    public struct AnonymousUnion
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;

        [FieldOffset(0)]
        public KEYBDINPUT ki;

        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }
}