// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal unsafe struct IPersistFile
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x0B, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public HRESULT Save(string szFileName, bool fRemember)
    {
        fixed (char* pszFileName = szFileName)
        {
            return ThisPtr->Save((IPersistFile*)Unsafe.AsPointer(ref this), pszFileName, fRemember);
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IPersist.Vftbl IPersistVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IPersistFile*, HRESULT> IsDirty;
        internal readonly delegate* unmanaged[Stdcall]<IPersistFile*, PCWSTR, STGM, HRESULT> Load;
        internal readonly delegate* unmanaged[Stdcall]<IPersistFile*, PCWSTR, BOOL, HRESULT> Save;
        internal readonly delegate* unmanaged[Stdcall]<IPersistFile*, PCWSTR, HRESULT> SaveCompleted;
        internal readonly delegate* unmanaged[Stdcall]<IPersistFile*, PWSTR*, HRESULT> GetCurFile;
    }
}