// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;

namespace Snap.Hutao.Win32.UI.Shell;

internal unsafe struct IFileDialogEvents
{
    internal static Guid IID = new(2536837339u, 32127, 17707, 137, 117, 116, 168, 88, 40, 211, 84);

    private Vftbl* thisPtr;

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnFileOk;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, IShellItem*, HRESULT> OnFolderChanging;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnFolderChange;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnSelectionChange;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, IShellItem*, FDE_SHAREVIOLATION_RESPONSE*, HRESULT> OnShareViolation;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnTypeChange;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, IShellItem*, FDE_OVERWRITE_RESPONSE*, HRESULT> OnOverwrite;
    }
}