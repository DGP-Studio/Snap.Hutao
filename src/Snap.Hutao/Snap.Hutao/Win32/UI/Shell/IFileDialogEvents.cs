// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal readonly unsafe struct IFileDialogEvents
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xDB, 0x10, 0x35, 0x97, 0x7F, 0x7D, 0x2B, 0x45, 0x89, 0x75, 0x74, 0xA8, 0x58, 0x28, 0xD3, 0x54]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnFileOk;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, IShellItem*, HRESULT> OnFolderChanging;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnFolderChange;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnSelectionChange;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, IShellItem*, FDE_SHAREVIOLATION_RESPONSE*, HRESULT> OnShareViolation;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, HRESULT> OnTypeChange;
        internal readonly delegate* unmanaged[Stdcall]<IFileDialogEvents*, IFileDialog*, IShellItem*, FDE_OVERWRITE_RESPONSE*, HRESULT> OnOverwrite;
    }
}