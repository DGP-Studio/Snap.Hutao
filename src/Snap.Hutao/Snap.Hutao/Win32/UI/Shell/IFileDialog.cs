// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell.Common;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal static unsafe class IFileDialog
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x36, 0x51, 0xF8, 0x42, 0x7E, 0xDB, 0x9C, 0x43, 0x85, 0xF1, 0xE4, 0x07, 0x5D, 0x13, 0x5F, 0xC8]);
    }

    public static HRESULT GetResult(this ObjectReference<Vftbl> objRef, out ObjectReference<IShellItem.Vftbl> si)
    {
        nint psi = default;
        HRESULT hr = objRef.Vftbl.GetResult(objRef.ThisPtr, &psi);
        si = ObjectReference<IShellItem.Vftbl>.Attach(ref psi, IShellItem.IID);
        return hr;
    }

    public static HRESULT SetFileName(this ObjectReference<Vftbl> objRef, string szName)
    {
        fixed (char* pszName = szName)
        {
            return objRef.Vftbl.SetFileName(objRef.ThisPtr, pszName);
        }
    }

    public static HRESULT SetFileTypes(this ObjectReference<Vftbl> objRef, ReadOnlySpan<COMDLG_FILTERSPEC> filterSpecs)
    {
        fixed (COMDLG_FILTERSPEC* rgFilterSpec = filterSpecs)
        {
            return objRef.Vftbl.SetFileTypes(objRef.ThisPtr, (uint)filterSpecs.Length, rgFilterSpec);
        }
    }

    public static HRESULT SetFolder(this ObjectReference<Vftbl> objRef, ObjectReference<IShellItem.Vftbl> si)
    {
        return objRef.Vftbl.SetFolder(objRef.ThisPtr, si.ThisPtr);
    }

    public static HRESULT SetOptions(this ObjectReference<Vftbl> objRef, FILEOPENDIALOGOPTIONS fos)
    {
        return objRef.Vftbl.SetOptions(objRef.ThisPtr, fos);
    }

    public static HRESULT SetTitle(this ObjectReference<Vftbl> objRef, string szTitle)
    {
        fixed (char* pszTitle = szTitle)
        {
            return objRef.Vftbl.SetTitle(objRef.ThisPtr, pszTitle);
        }
    }

    public static HRESULT Show(this ObjectReference<Vftbl> objRef, [Optional] HWND hwndOwner)
    {
        return objRef.Vftbl.IModalWindowVftbl.Show(objRef.ThisPtr, hwndOwner);
    }

    internal readonly struct Vftbl
    {
        internal readonly IModalWindow.Vftbl IModalWindowVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, COMDLG_FILTERSPEC*, HRESULT> SetFileTypes;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> SetFileTypeIndex;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint*, HRESULT> GetFileTypeIndex;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint*, HRESULT> Advise;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> Unadvise;
        internal readonly delegate* unmanaged[Stdcall]<nint, FILEOPENDIALOGOPTIONS, HRESULT> SetOptions;
        internal readonly delegate* unmanaged[Stdcall]<nint, FILEOPENDIALOGOPTIONS*, HRESULT> GetOptions;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> SetDefaultFolder;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> SetFolder;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> GetFolder;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> GetCurrentSelection;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> SetFileName;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR*, HRESULT> GetFileName;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> SetTitle;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> SetOkButtonLabel;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> SetFileNameLabel;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> GetResult;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, FDAP, HRESULT> AddPlace;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> SetDefaultExtension;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT, HRESULT> Close;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, HRESULT> SetClientGuid;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> ClearClientData;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> SetFilter;
    }
}