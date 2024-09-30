// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal static unsafe class IFileOperationProgressSink
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xA7, 0xF1, 0xB0, 0x04, 0x90, 0x94, 0xBC, 0x44, 0x96, 0xE1, 0x42, 0x96, 0xA3, 0x12, 0x52, 0xE2]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> StartOperations;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT, HRESULT> FinishOperations;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, PCWSTR, HRESULT> PreRenameItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, PCWSTR, HRESULT, nint, HRESULT> PostRenameItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, nint, PCWSTR, HRESULT> PreMoveItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, nint, PCWSTR, HRESULT, nint, HRESULT> PostMoveItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, nint, PCWSTR, HRESULT> PreCopyItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, nint, PCWSTR, HRESULT, nint, HRESULT> PostCopyItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, HRESULT> PreDeleteItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, HRESULT, nint, HRESULT> PostDeleteItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, PCWSTR, HRESULT> PreNewItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, PCWSTR, PCWSTR, uint, HRESULT, nint, HRESULT> PostNewItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, HRESULT> UpdateProgress;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> ResetTimer;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> PauseTimer;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> ResumeTimer;
    }
}