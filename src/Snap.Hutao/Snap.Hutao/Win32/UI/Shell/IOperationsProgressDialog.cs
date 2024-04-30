// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell.PropertiesSystem;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
[Guid("0C9FB851-E5C9-43EB-A370-F0677B13874C")]
internal unsafe struct IOperationsProgressDialog
{
    public readonly Vftbl* ThisPtr;

    internal static unsafe ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x51, 0xB8, 0x9F, 0x0C, 0xC9, 0xE5, 0xEB, 0x43, 0xA3, 0x70, 0xF0, 0x67, 0x7B, 0x13, 0x87, 0x4C];
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
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, HWND, uint, HRESULT> StartProgressDialog;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, HRESULT> StopProgressDialog;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, SPACTION, HRESULT> SetOperation;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, uint, HRESULT> SetMode;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, ulong, ulong, ulong, ulong, ulong, ulong, HRESULT> UpdateProgress;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, IShellItem*, IShellItem*, IShellItem*, HRESULT> UpdateLocations;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, HRESULT> ResetTimer;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, HRESULT> PauseTimer;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, HRESULT> ResumeTimer;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, ulong*, ulong*, HRESULT> GetMilliseconds;
        internal readonly delegate* unmanaged[Stdcall]<IOperationsProgressDialog*, PDOPSTATUS*, HRESULT> GetOperationStatus;
    }
}