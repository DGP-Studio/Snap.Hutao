// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell.Common;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SuppressMessage("", "SH002")]
[SupportedOSPlatform("windows6.0.6000")]
internal unsafe struct IFileDialog
{
    internal static Guid IID = new(1123569974u, 56190, 17308, 133, 241, 228, 7, 93, 19, 95, 200);

    private Vftbl* thisPtr;

    public unsafe HRESULT QueryInterface<TInterface>(ref readonly Guid riid, out TInterface* pvObject)
        where TInterface : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (TInterface** ppvObject = &pvObject)
            {
                return thisPtr->IModalWindowVftbl.IUnknownVftbl.QueryInterface((IUnknown*)Unsafe.AsPointer(ref this), riid2, (void**)ppvObject);
            }
        }
    }

    public uint AddRef()
    {
        return thisPtr->IModalWindowVftbl.IUnknownVftbl.AddRef((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public uint Release()
    {
        return thisPtr->IModalWindowVftbl.IUnknownVftbl.Release((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public HRESULT Show([AllowNull] HWND hwndOwner)
    {
        return thisPtr->IModalWindowVftbl.Show((IModalWindow*)Unsafe.AsPointer(ref this), hwndOwner);
    }

    public HRESULT SetFileTypes(ReadOnlySpan<COMDLG_FILTERSPEC> filterSpecs)
    {
        fixed (COMDLG_FILTERSPEC* rgFilterSpec = filterSpecs)
        {
            return thisPtr->SetFileTypes((IFileDialog*)Unsafe.AsPointer(ref this), (uint)filterSpecs.Length, rgFilterSpec);
        }
    }

    public HRESULT SetOptions(FILEOPENDIALOGOPTIONS fos)
    {
        return thisPtr->SetOptions((IFileDialog*)Unsafe.AsPointer(ref this), fos);
    }

    public HRESULT SetFolder(IShellItem* si)
    {
        return thisPtr->SetFolder((IFileDialog*)Unsafe.AsPointer(ref this), si);
    }

    public HRESULT SetFileName(string szName)
    {
        fixed (char* pszName = szName)
        {
            return thisPtr->SetFileName((IFileDialog*)Unsafe.AsPointer(ref this), pszName);
        }
    }

    public HRESULT SetTitle(string szTitle)
    {
        fixed (char* pszTitle = szTitle)
        {
            return thisPtr->SetTitle((IFileDialog*)Unsafe.AsPointer(ref this), pszTitle);
        }
    }

    public HRESULT GetResult(out IShellItem* psi)
    {
        fixed (IShellItem** ppsi = &psi)
        {
            return thisPtr->GetResult((IFileDialog*)Unsafe.AsPointer(ref this), ppsi);
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