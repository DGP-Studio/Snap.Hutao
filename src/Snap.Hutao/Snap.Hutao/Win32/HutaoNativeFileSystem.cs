// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeFileSystem
{
    private readonly ObjectReference<Vftbl> objRef;
    private readonly ObjectReference<Vftbl2>? objRef2;
    private readonly ObjectReference<Vftbl3>? objRef3;
    private readonly ObjectReference<Vftbl4>? objRef4;

    public HutaoNativeFileSystem(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
        objRef.TryAs(typeof(Vftbl3).GUID, out objRef3);
        objRef.TryAs(typeof(Vftbl4).GUID, out objRef4);
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

    public void CreateLink(ReadOnlySpan<char> fileLocation, ReadOnlySpan<char> arguments, ReadOnlySpan<char> iconLocation, ReadOnlySpan<char> fileName)
    {
        HutaoException.ThrowIf(objRef2 is null, "IHutaoFileSystem2 is not supported");
        fixed (char* pFileLocation = fileLocation)
        {
            fixed (char* pArguments = arguments)
            {
                fixed (char* pIconLocation = iconLocation)
                {
                    fixed (char* pFileName = fileName)
                    {
                        Marshal.ThrowExceptionForHR(objRef2.Vftbl.CreateLink(objRef2.ThisPtr, pFileLocation, pArguments, pIconLocation, pFileName));
                    }
                }
            }
        }
    }

    public BOOL PickFile(HWND hwnd, ReadOnlySpan<char> title, ReadOnlySpan<char> defaultFileName, ReadOnlySpan<char> fileFilterName, ReadOnlySpan<char> fileFilterType, out string? path)
    {
        HutaoException.ThrowIf(objRef3 is null, "IHutaoFileSystem3 is not supported");
        fixed (char* pTitle = title)
        {
            fixed (char* pDefaultFileName = defaultFileName)
            {
                fixed (char* pFileFilterName = fileFilterName)
                {
                    fixed (char* pFileFilterType = fileFilterType)
                    {
                        BOOL picked;
                        nint pPath = default;
                        Marshal.ThrowExceptionForHR(objRef3!.Vftbl.PickFile(objRef3.ThisPtr, hwnd, pTitle, pDefaultFileName, pFileFilterName, pFileFilterType, &picked, (HutaoString.Vftbl**)&pPath));
                        path = HutaoString.AttachAbi(ref pPath).Value;
                        return picked;
                    }
                }
            }
        }
    }

    public BOOL SaveFile(HWND hwnd, ReadOnlySpan<char> title, ReadOnlySpan<char> defaultFileName, ReadOnlySpan<char> fileFilterName, ReadOnlySpan<char> fileFilterType, out string? path)
    {
        HutaoException.ThrowIf(objRef3 is null, "IHutaoFileSystem3 is not supported");
        fixed (char* pTitle = title)
        {
            fixed (char* pDefaultFileName = defaultFileName)
            {
                fixed (char* pFileFilterName = fileFilterName)
                {
                    fixed (char* pFileFilterType = fileFilterType)
                    {
                        BOOL picked;
                        nint pPath = default;
                        Marshal.ThrowExceptionForHR(objRef3!.Vftbl.SaveFile(objRef3.ThisPtr, hwnd, pTitle, pDefaultFileName, pFileFilterName, pFileFilterType, &picked, (HutaoString.Vftbl**)&pPath));
                        path = HutaoString.AttachAbi(ref pPath).Value;
                        return picked;
                    }
                }
            }
        }
    }

    public BOOL PickFolder(HWND hwnd, ReadOnlySpan<char> title, out string? path)
    {
        HutaoException.ThrowIf(objRef3 is null, "IHutaoFileSystem3 is not supported");
        fixed (char* pTitle = title)
        {
            BOOL picked;
            nint pPath = default;
            Marshal.ThrowExceptionForHR(objRef3!.Vftbl.PickFolder(objRef3.ThisPtr, hwnd, pTitle, &picked, (HutaoString.Vftbl**)&pPath));
            path = HutaoString.AttachAbi(ref pPath).Value;
            return picked;
        }
    }

    public void CopyFileAllowDecryptedDestination(ReadOnlySpan<char> existingFileName, ReadOnlySpan<char> newFileName, BOOL overwrite)
    {
        HutaoException.ThrowIf(objRef4 is null, "IHutaoFileSystem4 is not supported");
        fixed (char* pExistingFileName = existingFileName)
        {
            fixed (char* pNewFileName = newFileName)
            {
                Marshal.ThrowExceptionForHR(objRef4!.Vftbl.CopyFileAllowDecryptedDestination(objRef4.ThisPtr, pExistingFileName, pNewFileName, overwrite));
            }
        }
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeFileSystem)]
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

    [Guid(HutaoNativeMethods.IID_IHutaoNativeFileSystem2)]
    private readonly struct Vftbl2
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, PCWSTR, PCWSTR, HRESULT> CreateLink;
#pragma warning restore CS0649
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeFileSystem3)]
    private readonly struct Vftbl3
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, PCWSTR, PCWSTR, PCWSTR, PCWSTR, BOOL*, HutaoString.Vftbl**, HRESULT> PickFile;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, PCWSTR, PCWSTR, PCWSTR, PCWSTR, BOOL*, HutaoString.Vftbl**, HRESULT> SaveFile;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, PCWSTR, BOOL*, HutaoString.Vftbl**, HRESULT> PickFolder;
#pragma warning restore CS0649
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeFileSystem4)]
    private readonly struct Vftbl4
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, BOOL, HRESULT> CopyFileAllowDecryptedDestination;
#pragma warning restore CS0649
    }
}