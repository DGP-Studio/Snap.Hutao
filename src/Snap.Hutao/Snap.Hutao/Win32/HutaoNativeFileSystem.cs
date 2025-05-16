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
    private readonly ObjectReference<Vftbl2>? objRef2;

    public HutaoNativeFileSystem(ObjectReference<Vftbl> objRef)
    {
        ObjRef = objRef;
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
    }

    private ObjectReference<Vftbl> ObjRef { get; }

    private ObjectReference<Vftbl2>? ObjRef2 { get=> objRef2; }

    public void RenameItem(ReadOnlySpan<char> filePath, ReadOnlySpan<char> newName)
    {
        fixed (char* pFilePath = filePath)
        {
            fixed (char* pNewName = newName)
            {
                Marshal.ThrowExceptionForHR(ObjRef.Vftbl.RenameItem(ObjRef.ThisPtr, pFilePath, pNewName));
            }
        }
    }

    public void RenameItemWithOptions(ReadOnlySpan<char> filePath, ReadOnlySpan<char> newName, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pFilePath = filePath)
        {
            fixed (char* pNewName = newName)
            {
                Marshal.ThrowExceptionForHR(ObjRef.Vftbl.RenameItemWithOptions(ObjRef.ThisPtr, pFilePath, pNewName, flags));
            }
        }
    }

    public void MoveItem(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MoveItem(ObjRef.ThisPtr, pOldPath, pNewFolder));
            }
        }
    }

    public void MoveItemWithOptions(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MoveItemWithOptions(ObjRef.ThisPtr, pOldPath, pNewFolder, flags));
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
                    Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MoveItemWithName(ObjRef.ThisPtr, pOldPath, pNewFolder, pName));
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
                    Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MoveItemWithNameAndOptions(ObjRef.ThisPtr, pOldPath, pNewFolder, pName, flags));
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
                Marshal.ThrowExceptionForHR(ObjRef.Vftbl.CopyItem(ObjRef.ThisPtr, pOldPath, pNewFolder));
            }
        }
    }

    public void CopyItemWithOptions(ReadOnlySpan<char> oldPath, ReadOnlySpan<char> newFolder, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pOldPath = oldPath)
        {
            fixed (char* pNewFolder = newFolder)
            {
                Marshal.ThrowExceptionForHR(ObjRef.Vftbl.CopyItemWithOptions(ObjRef.ThisPtr, pOldPath, pNewFolder, flags));
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
                    Marshal.ThrowExceptionForHR(ObjRef.Vftbl.CopyItemWithName(ObjRef.ThisPtr, pOldPath, pNewFolder, pName));
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
                    Marshal.ThrowExceptionForHR(ObjRef.Vftbl.CopyItemWithNameAndOptions(ObjRef.ThisPtr, pOldPath, pNewFolder, pName, flags));
                }
            }
        }
    }

    public void DeleteItem(ReadOnlySpan<char> filePath)
    {
        fixed (char* pFilePath = filePath)
        {
            Marshal.ThrowExceptionForHR(ObjRef.Vftbl.DeleteItem(ObjRef.ThisPtr, pFilePath));
        }
    }

    public void DeleteItemWithOptions(ReadOnlySpan<char> filePath, FILEOPERATION_FLAGS flags)
    {
        fixed (char* pFilePath = filePath)
        {
            Marshal.ThrowExceptionForHR(ObjRef.Vftbl.DeleteItemWithOptions(ObjRef.ThisPtr, pFilePath, flags));
        }
    }

    public void CreateLink(ReadOnlySpan<char> fileLocation, ReadOnlySpan<char> arguments, ReadOnlySpan<char> iconLocation, ReadOnlySpan<char> fileName)
    {
        fixed (char* pFileLocation = fileLocation)
        {
            fixed (char* pArguments = arguments)
            {
                fixed (char* pIconLocation = iconLocation)
                {
                    fixed (char* pFileName = fileName)
                    {
                        Marshal.ThrowExceptionForHR(ObjRef2!.Vftbl.CreateLink(ObjRef2.ThisPtr, pFileLocation, pArguments, pIconLocation, pFileName));
                    }
                }
            }
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

    [Guid("62616943-38e6-4bbb-84d1-dab847cb2145")]
    private readonly struct Vftbl2
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, PCWSTR, PCWSTR, HRESULT> CreateLink;
#pragma warning restore CS0649
    }
}