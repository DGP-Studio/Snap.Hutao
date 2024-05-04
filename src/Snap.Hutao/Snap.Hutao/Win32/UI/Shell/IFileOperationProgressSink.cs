// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal unsafe readonly struct IFileOperationProgressSink
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xA7, 0xF1, 0xB0, 0x04, 0x90, 0x94, 0xBC, 0x44, 0x96, 0xE1, 0x42, 0x96, 0xA3, 0x12, 0x52, 0xE2];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, HRESULT> StartOperations;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, HRESULT, HRESULT> FinishOperations;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, PCWSTR, HRESULT> PreRenameItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, PCWSTR, HRESULT, IShellItem*, HRESULT> PostRenameItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, IShellItem*, PCWSTR, HRESULT> PreMoveItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, IShellItem*, PCWSTR, HRESULT, IShellItem*, HRESULT> PostMoveItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, IShellItem*, PCWSTR, HRESULT> PreCopyItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, IShellItem*, PCWSTR, HRESULT, IShellItem*, HRESULT> PostCopyItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, HRESULT> PreDeleteItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, HRESULT, IShellItem*, HRESULT> PostDeleteItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, PCWSTR, HRESULT> PreNewItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, IShellItem*, PCWSTR, PCWSTR, uint, HRESULT, IShellItem*, HRESULT> PostNewItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, uint, uint, HRESULT> UpdateProgress;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, HRESULT> ResetTimer;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, HRESULT> PauseTimer;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperationProgressSink*, HRESULT> ResumeTimer;
    }
}