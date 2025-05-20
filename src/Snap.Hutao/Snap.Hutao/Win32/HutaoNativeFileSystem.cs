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
    private readonly ObjectReference<Vftbl2>? objRef2;
    private readonly ObjectReference<Vftbl3>? objRef3;

    public HutaoNativeFileSystem(ObjectReference<Vftbl> objRef)
    {
        ObjRef = objRef;
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
        objRef.TryAs(typeof(Vftbl3).GUID, out objRef3);
    }

    private ObjectReference<Vftbl> ObjRef { get; }

    private ObjectReference<Vftbl2>? ObjRef2 { get => objRef2; }

    private ObjectReference<Vftbl3>? ObjRef3 { get => objRef3; }

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
        HutaoException.ThrowIf(ObjRef2 is null, "IHutaoFileSystem2 is not supported");
        fixed (char* pFileLocation = fileLocation)
        {
            fixed (char* pArguments = arguments)
            {
                fixed (char* pIconLocation = iconLocation)
                {
                    fixed (char* pFileName = fileName)
                    {
                        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.CreateLink(ObjRef2.ThisPtr, pFileLocation, pArguments, pIconLocation, pFileName));
                    }
                }
            }
        }
    }

    public BOOL PickFile(HWND hwnd, ReadOnlySpan<char> title, ReadOnlySpan<char> defaultFileName, ReadOnlySpan<char> fileFilterName, ReadOnlySpan<char> fileFilterType, out string? path)
    {
        HutaoException.ThrowIf(ObjRef3 is null, "IHutaoFileSystem3 is not supported");
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
                        Marshal.ThrowExceptionForHR(ObjRef3!.Vftbl.PickFile(ObjRef3.ThisPtr, hwnd, pTitle, pDefaultFileName, pFileFilterName, pFileFilterType, &picked, (HutaoString.Vftbl**)&pPath));
                        path = HutaoString.AttachAbi(ref pPath).Get();
                        return picked;
                    }
                }
            }
        }
    }

    public BOOL SaveFile(HWND hwnd, ReadOnlySpan<char> title, ReadOnlySpan<char> defaultFileName, ReadOnlySpan<char> fileFilterName, ReadOnlySpan<char> fileFilterType, out string? path)
    {
        HutaoException.ThrowIf(ObjRef3 is null, "IHutaoFileSystem3 is not supported");
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
                        Marshal.ThrowExceptionForHR(ObjRef3!.Vftbl.SaveFile(ObjRef3.ThisPtr, hwnd, pTitle, pDefaultFileName, pFileFilterName, pFileFilterType, &picked, (HutaoString.Vftbl**)&pPath));
                        path = HutaoString.AttachAbi(ref pPath).Get();
                        return picked;
                    }
                }
            }
        }
    }

    public BOOL PickFolder(HWND hwnd, ReadOnlySpan<char> title, out string? path)
    {
        HutaoException.ThrowIf(ObjRef3 is null, "IHutaoFileSystem3 is not supported");
        fixed (char* pTitle = title)
        {
            BOOL picked;
            nint pPath = default;
            Marshal.ThrowExceptionForHR(ObjRef3!.Vftbl.PickFolder(ObjRef3.ThisPtr, hwnd, pTitle, &picked, (HutaoString.Vftbl**)&pPath));
            path = HutaoString.AttachAbi(ref pPath).Get();
            return picked;
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
}