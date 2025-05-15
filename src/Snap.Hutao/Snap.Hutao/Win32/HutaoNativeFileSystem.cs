// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeFileSystem
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeFileSystem(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public void RenameItem(ReadOnlySpan<char> filePath, ReadOnlySpan<char> newName)
    {
        fixed (char* pFilePath = filePath)
        {
            fixed (char* pNewName = newName)
            {
                Marshal.ThrowExceptionForHR(objRef.Vftbl.RenameItem(objRef.ThisPtr, pFilePath, pNewName));
            }
        }
    }

    public void RenameItemWithOptions(ReadOnlySpan<char> filePath, ReadOnlySpan<char> newName, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pFilePath = filePath)
        {
            fixed (char* pNewName = newName)
            {
                Marshal.ThrowExceptionForHR(objRef.Vftbl.RenameItemWithOptions(objRef.ThisPtr, pFilePath, pNewName, flags));
            }
        }
    }

    public void MoveItem(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                Marshal.ThrowExceptionForHR(objRef.Vftbl.MoveItem(objRef.ThisPtr, pOldPath, pNewFolder));
            }
        }
    }

    public void MoveItemWithOptions(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                Marshal.ThrowExceptionForHR(objRef.Vftbl.MoveItemWithOptions(objRef.ThisPtr, pOldPath, pNewFolder, flags));
            }
        }
    }

    public void MoveItemWithName(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder, ReadOnlySpan<char> name)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                fixed (char* pName = name)
                {
                    Marshal.ThrowExceptionForHR(objRef.Vftbl.MoveItemWithName(objRef.ThisPtr, pOldPath, pNewFolder, pName));
                }
            }
        }
    }

    public void MoveItemWithNameAndOptions(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder, ReadOnlySpan<char> name, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                fixed (char* pName = name)
                {
                    Marshal.ThrowExceptionForHR(objRef.Vftbl.MoveItemWithNameAndOptions(objRef.ThisPtr, pOldPath, pNewFolder, pName, flags));
                }
            }
        }
    }

    public void CopyItem(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                Marshal.ThrowExceptionForHR(objRef.Vftbl.CopyItem(objRef.ThisPtr, pOldPath, pNewFolder));
            }
        }
    }

    public void CopyItemWithOptions(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                Marshal.ThrowExceptionForHR(objRef.Vftbl.CopyItemWithOptions(objRef.ThisPtr, pOldPath, pNewFolder, flags));
            }
        }
    }

    public void CopyItemWithName(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder, ReadOnlySpan<char> name)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                fixed (char* pName = name)
                {
                    Marshal.ThrowExceptionForHR(objRef.Vftbl.CopyItemWithName(objRef.ThisPtr, pOldPath, pNewFolder, pName));
                }
            }
        }
    }

    public void CopyItemWithNameAndOptions(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder, ReadOnlySpan<char> name, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                fixed (char* pName = name)
                {
                    Marshal.ThrowExceptionForHR(objRef.Vftbl.CopyItemWithNameAndOptions(objRef.ThisPtr, pOldPath, pNewFolder, pName, flags));
                }
            }
        }
    }

    public void DeleteItem(ReadOnlySpan<char> filePath)
    {
        fixed (char* pFilePath = filePath)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.DeleteItem(objRef.ThisPtr, pFilePath));
        }
    }

    public void DeleteItemWithOptions(ReadOnlySpan<char> filePath, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pFilePath = filePath)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.DeleteItemWithOptions(objRef.ThisPtr, pFilePath, flags));
        }
    }

    [Guid("fdd58117-0c7f-44d8-a7a2-8b1766474a93")]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, HRESULT> RenameItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, FILEOPERATION_FLAGS, HRESULT> RenameItemWithOptions;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, HRESULT> MoveItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, FILEOPERATION_FLAGS, HRESULT> MoveItemWithOptions;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, PCWSTR, HRESULT> MoveItemWithName;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, PCWSTR, FILEOPERATION_FLAGS, HRESULT> MoveItemWithNameAndOptions;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, HRESULT> CopyItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, FILEOPERATION_FLAGS, HRESULT> CopyItemWithOptions;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, PCWSTR, HRESULT> CopyItemWithName;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, PCWSTR, FILEOPERATION_FLAGS, HRESULT> CopyItemWithNameAndOptions;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> DeleteItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, FILEOPERATION_FLAGS, HRESULT> DeleteItemWithOptions;
#pragma warning restore CS0649
    }
}