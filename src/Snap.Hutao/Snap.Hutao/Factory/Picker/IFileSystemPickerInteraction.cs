// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Factory.Picker;

internal interface IFileSystemPickerInteraction
{
}

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IFileSystemPickerInteraction))]
internal sealed partial class FileSystemPickerInteraction : IFileSystemPickerInteraction
{
    private readonly ICurrentWindowReference currentWindowReference;

    public unsafe string PickFile()
    {
        HRESULT result = CoCreateInstance<FileOpenDialog, IFileOpenDialog>(default, CLSCTX.CLSCTX_INPROC_SERVER, out IFileOpenDialog dialog);
        Marshal.ThrowExceptionForHR(result);

        dialog.Show(currentWindowReference.GetWindowHandle());
        dialog.GetResult(out IShellItem item);
        PWSTR name = default;
        string file;
        try
        {
            item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out name);
            file = new((char*)name);
        }
        finally
        {
            Marshal.FreeCoTaskMem((nint)name.Value);
        }

        return file;
    }
}