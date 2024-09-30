// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal static unsafe class IPersistFile
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x0B, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    public static HRESULT Save(this ObjectReference<Vftbl> objRef, ReadOnlySpan<char> szFileName, bool fRemember)
    {
        fixed (char* pszFileName = szFileName)
        {
            return objRef.Vftbl.Save(objRef.ThisPtr, pszFileName, fRemember);
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IPersist.Vftbl IPersistVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> IsDirty;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, STGM, HRESULT> Load;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, BOOL, HRESULT> Save;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> SaveCompleted;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR*, HRESULT> GetCurFile;
    }
}