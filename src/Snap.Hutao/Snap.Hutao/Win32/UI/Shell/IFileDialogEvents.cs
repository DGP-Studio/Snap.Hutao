// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
[Guid("973510DB-7D7F-452B-8975-74A85828D354")]
internal unsafe struct IFileDialogEvents
{
    public readonly Vftbl* ThisPtr;

    internal static unsafe ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xDB, 0x10, 0x35, 0x97, 0x7F, 0x7D, 0x2B, 0x45, 0x89, 0x75, 0x74, 0xA8, 0x58, 0x28, 0xD3, 0x54];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public unsafe HRESULT QueryInterface<TInterface>(ref readonly Guid riid, out TInterface* pvObject)
        where TInterface : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (TInterface** ppvObject = &pvObject)
            {
                return ThisPtr->IUnknownVftbl.QueryInterface((IUnknown*)Unsafe.AsPointer(ref this), riid2, (void**)ppvObject);
            }
        }
    }

    public uint AddRef()
    {
        return ThisPtr->IUnknownVftbl.AddRef((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public uint Release()
    {
        return ThisPtr->IUnknownVftbl.Release((IUnknown*)Unsafe.AsPointer(ref this));
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnFileOk;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, IShellItem*, HRESULT> OnFolderChanging;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnFolderChange;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnSelectionChange;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, IShellItem*, FDE_SHAREVIOLATION_RESPONSE*, HRESULT> OnShareViolation;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnTypeChange;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, IShellItem*, FDE_OVERWRITE_RESPONSE*, HRESULT> OnOverwrite;
    }
}