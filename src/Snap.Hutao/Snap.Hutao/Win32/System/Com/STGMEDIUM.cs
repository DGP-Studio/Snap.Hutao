// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Gdi;
using Snap.Hutao.Win32.System.Com.StructuredStorage;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace Snap.Hutao.Win32.System.Com;

[SuppressMessage("", "SA1307")]
internal struct STGMEDIUM
{
    // TYMED
    public uint tymed;
    public Union u;
    public unsafe IUnknownVftbl* pUnkForRelease;

    [StructLayout(LayoutKind.Explicit)]
    internal struct Union
    {
        [FieldOffset(0)]
        public HBITMAP hBitmap;

        [FieldOffset(0)]
        public unsafe void* hMetaFilePict;

        [FieldOffset(0)]
        public HENHMETAFILE hEnhMetaFile;

        [FieldOffset(0)]
        public HGLOBAL hGlobal;

        [FieldOffset(0)]
        public PWSTR lpszFileName;

        [FieldOffset(0)]
        public unsafe IStream.Vftbl* pstm;

        [FieldOffset(0)]
        public unsafe IStorage.Vftbl* pstg;
    }
}