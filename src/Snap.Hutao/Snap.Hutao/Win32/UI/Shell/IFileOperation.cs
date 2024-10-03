// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal static unsafe class IFileOperation
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x5F, 0xAB, 0x7A, 0x94, 0x5C, 0x0A, 0x13, 0x4C, 0xB4, 0xD6, 0x4B, 0xF7, 0x83, 0x6F, 0xC9, 0xF8]);
    }

    public static HRESULT SetOperationFlags(this ObjectReference<Vftbl> objRef, FILEOPERATION_FLAGS dwOperationFlags)
    {
        return objRef.Vftbl.SetOperationFlags(objRef.ThisPtr, dwOperationFlags);
    }

    public static HRESULT RenameItem(this ObjectReference<Vftbl> objRef, ObjectReference<IShellItem.Vftbl> siItem, ReadOnlySpan<char> szNewName, ObjectReference<IFileOperationProgressSink.Vftbl> fopsItem)
    {
        fixed (char* pszNewName = szNewName)
        {
            return objRef.Vftbl.RenameItem(objRef.ThisPtr, siItem.ThisPtr, pszNewName, fopsItem?.ThisPtr ?? 0);
        }
    }

    public static HRESULT MoveItem(this ObjectReference<Vftbl> objRef, ObjectReference<IShellItem.Vftbl> siItem, ObjectReference<IShellItem.Vftbl> siDestinationFolder, [Optional] ReadOnlySpan<char> szNewName, ObjectReference<IFileOperationProgressSink.Vftbl> fopsItem)
    {
        fixed (char* pszNewName = szNewName)
        {
            return objRef.Vftbl.MoveItem(objRef.ThisPtr, siItem.ThisPtr, siDestinationFolder.ThisPtr, pszNewName, fopsItem?.ThisPtr ?? 0);
        }
    }

    public static HRESULT DeleteItem(this ObjectReference<Vftbl> objRef, ObjectReference<IShellItem.Vftbl> siItem, ObjectReference<IFileOperationProgressSink.Vftbl> fopsItem)
    {
        return objRef.Vftbl.DeleteItem(objRef.ThisPtr, siItem.ThisPtr, fopsItem?.ThisPtr ?? 0);
    }

    public static HRESULT PerformOperations(this ObjectReference<Vftbl> objRef)
    {
        return objRef.Vftbl.PerformOperations(objRef.ThisPtr);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint*, HRESULT> Advise;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> Unadvise;
        internal readonly delegate* unmanaged[Stdcall]<nint, FILEOPERATION_FLAGS, HRESULT> SetOperationFlags;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> SetProgressMessage;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> SetProgressDialog;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> SetProperties;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, HRESULT> SetOwnerWindow;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> ApplyPropertiesToItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> ApplyPropertiesToItems;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, PCWSTR, nint, HRESULT> RenameItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, PCWSTR, HRESULT> RenameItems;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, PCWSTR, nint, HRESULT> MoveItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, HRESULT> MoveItems;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, PCWSTR, nint, HRESULT> CopyItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, HRESULT> CopyItems;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, HRESULT> DeleteItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> DeleteItems;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, PCWSTR, PCWSTR, nint, HRESULT> NewItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> PerformOperations;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> GetAnyOperationsAborted;
    }
}