// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell.PropertiesSystem;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal unsafe struct IFileOperation
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x5F, 0xAB, 0x7A, 0x94, 0x5C, 0x0A, 0x13, 0x4C, 0xB4, 0xD6, 0x4B, 0xF7, 0x83, 0x6F, 0xC9, 0xF8];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public HRESULT SetOperationFlags(FILEOPERATION_FLAGS dwOperationFlags)
    {
        return ThisPtr->SetOperationFlags((IFileOperation*)Unsafe.AsPointer(ref this), dwOperationFlags);
    }

    public HRESULT RenameItem(ObjectReference<IShellItem.Vftbl> siItem, ReadOnlySpan<char> szNewName, ObjectReference<IFileOperationProgressSink.Vftbl> fopsItem)
    {
        fixed (char* pszNewName = szNewName)
        {
            return ThisPtr->RenameItem((IFileOperation*)Unsafe.AsPointer(ref this), (IShellItem*)siItem.ThisPtr, pszNewName, (IFileOperationProgressSink*)(fopsItem?.ThisPtr ?? 0));
        }
    }

    public HRESULT MoveItem(IShellItem* psiItem, ObjectReference<IShellItem.Vftbl> siDestinationFolder, [AllowNull] ReadOnlySpan<char> szNewName, ObjectReference<IFileOperationProgressSink.Vftbl> fopsItem)
    {
        fixed (char* pszNewName = szNewName)
        {
            return ThisPtr->MoveItem((IFileOperation*)Unsafe.AsPointer(ref this), psiItem, (IShellItem*)siDestinationFolder.ThisPtr, pszNewName, (IFileOperationProgressSink*)(fopsItem?.ThisPtr ?? 0));
        }
    }

    public HRESULT DeleteItem(IShellItem* psiItem, ObjectReference<IFileOperationProgressSink.Vftbl> fopsItem)
    {
        return ThisPtr->DeleteItem((IFileOperation*)Unsafe.AsPointer(ref this), psiItem, (IFileOperationProgressSink*)(fopsItem?.ThisPtr ?? 0));
    }

    public HRESULT PerformOperations()
    {
        return ThisPtr->PerformOperations((IFileOperation*)Unsafe.AsPointer(ref this));
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IFileOperationProgressSink*, uint*, HRESULT> Advise;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, uint, HRESULT> Unadvise;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, FILEOPERATION_FLAGS, HRESULT> SetOperationFlags;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, PCWSTR, HRESULT> SetProgressMessage;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IOperationsProgressDialog*, HRESULT> SetProgressDialog;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IPropertyChangeArray*, HRESULT> SetProperties;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, HWND, HRESULT> SetOwnerWindow;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IShellItem*, HRESULT> ApplyPropertiesToItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IUnknown*, HRESULT> ApplyPropertiesToItems;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IShellItem*, PCWSTR, IFileOperationProgressSink*, HRESULT> RenameItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IUnknown*, PCWSTR, HRESULT> RenameItems;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IShellItem*, IShellItem*, PCWSTR, IFileOperationProgressSink*, HRESULT> MoveItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IUnknown*, IShellItem*, HRESULT> MoveItems;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IShellItem*, IShellItem*, PCWSTR, IFileOperationProgressSink*, HRESULT> CopyItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IUnknown*, IShellItem*, HRESULT> CopyItems;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IShellItem*, IFileOperationProgressSink*, HRESULT> DeleteItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IUnknown*, HRESULT> DeleteItems;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, IShellItem*, uint, PCWSTR, PCWSTR, IFileOperationProgressSink*, HRESULT> NewItem;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, HRESULT> PerformOperations;
        internal readonly delegate* unmanaged[Stdcall]<IFileOperation*, BOOL*, HRESULT> GetAnyOperationsAborted;
    }
}