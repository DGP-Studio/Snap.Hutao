// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal unsafe struct IFileDialog
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x36, 0x51, 0xF8, 0x42, 0x7E, 0xDB, 0x9C, 0x43, 0x85, 0xF1, 0xE4, 0x07, 0x5D, 0x13, 0x5F, 0xC8];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public HRESULT Show([AllowNull] HWND hwndOwner)
    {
        return ThisPtr->IModalWindowVftbl.Show((IModalWindow*)Unsafe.AsPointer(ref this), hwndOwner);
    }

    public HRESULT SetFileTypes(ReadOnlySpan<COMDLG_FILTERSPEC> filterSpecs)
    {
        fixed (COMDLG_FILTERSPEC* rgFilterSpec = filterSpecs)
        {
            return ThisPtr->SetFileTypes((IFileDialog*)Unsafe.AsPointer(ref this), (uint)filterSpecs.Length, rgFilterSpec);
        }
    }

    public HRESULT SetOptions(FILEOPENDIALOGOPTIONS fos)
    {
        return ThisPtr->SetOptions((IFileDialog*)Unsafe.AsPointer(ref this), fos);
    }

    public HRESULT SetFolder(IShellItem* si)
    {
        return ThisPtr->SetFolder((IFileDialog*)Unsafe.AsPointer(ref this), si);
    }

    public HRESULT SetFileName(string szName)
    {
        fixed (char* pszName = szName)
        {
            return ThisPtr->SetFileName((IFileDialog*)Unsafe.AsPointer(ref this), pszName);
        }
    }

    public HRESULT SetTitle(string szTitle)
    {
        fixed (char* pszTitle = szTitle)
        {
            return ThisPtr->SetTitle((IFileDialog*)Unsafe.AsPointer(ref this), pszTitle);
        }
    }

    public HRESULT GetResult(out IShellItem* psi)
    {
        fixed (IShellItem** ppsi = &psi)
        {
            return ThisPtr->GetResult((IFileDialog*)Unsafe.AsPointer(ref this), ppsi);
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IModalWindow.Vftbl IModalWindowVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, uint, COMDLG_FILTERSPEC*, HRESULT> SetFileTypes;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, uint, HRESULT> SetFileTypeIndex;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, uint*, HRESULT> GetFileTypeIndex;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, IFileDialogEvents*, uint*, HRESULT> Advise;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, uint, HRESULT> Unadvise;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, FILEOPENDIALOGOPTIONS, HRESULT> SetOptions;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, FILEOPENDIALOGOPTIONS*, HRESULT> GetOptions;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, IShellItem*, HRESULT> SetDefaultFolder;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, IShellItem*, HRESULT> SetFolder;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, IShellItem**, HRESULT> GetFolder;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, IShellItem**, HRESULT> GetCurrentSelection;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, PCWSTR, HRESULT> SetFileName;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, PWSTR*, HRESULT> GetFileName;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, PCWSTR, HRESULT> SetTitle;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, PCWSTR, HRESULT> SetOkButtonLabel;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, PCWSTR, HRESULT> SetFileNameLabel;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, IShellItem**, HRESULT> GetResult;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, IShellItem*, FDAP, HRESULT> AddPlace;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, PCWSTR, HRESULT> SetDefaultExtension;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, HRESULT, HRESULT> Close;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, Guid*, HRESULT> SetClientGuid;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, HRESULT> ClearClientData;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialog*, IShellItemFilter*, HRESULT> SetFilter;
    }
}