// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.LifeCycle;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.Common;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Factory.Picker;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IFileSystemPickerInteraction))]
internal sealed partial class FileSystemPickerInteraction : IFileSystemPickerInteraction
{
    private readonly ICurrentWindowReference currentWindowReference;

    public unsafe ValueResult<bool, ValueFile> PickFile(string? title, string? defaultFileName, (string Name, string Type)[]? filters)
    {
        CoCreateInstance<FileOpenDialog, IFileOpenDialog>(default, CLSCTX.CLSCTX_INPROC_SERVER, out IFileOpenDialog dialog).ThrowOnFailure();

        FILEOPENDIALOGOPTIONS options =
            FILEOPENDIALOGOPTIONS.FOS_NOTESTFILECREATE |
            FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM |
            FILEOPENDIALOGOPTIONS.FOS_NOCHANGEDIR;

        dialog.SetOptions(options);
        SetDesktopAsStartupFolder(dialog);

        if (!string.IsNullOrEmpty(title))
        {
            dialog.SetTitle(title);
        }

        if (!string.IsNullOrEmpty(defaultFileName))
        {
            dialog.SetFileName(defaultFileName);
        }

        if (filters is { Length: > 0 })
        {
            SetFileTypes(dialog, filters);
        }

        HRESULT res = dialog.Show(currentWindowReference.GetWindowHandle());
        if (res == HRESULT_FROM_WIN32(WIN32_ERROR.ERROR_CANCELLED))
        {
            return new(false, default);
        }
        else
        {
            Marshal.ThrowExceptionForHR(res);
        }

        dialog.GetResult(out IShellItem item);

        PWSTR displayName = default;
        string file;
        try
        {
            item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out displayName);
            file = new((char*)displayName);
        }
        finally
        {
            Marshal.FreeCoTaskMem((nint)displayName.Value);
        }

        return new(true, file);
    }

    public unsafe ValueResult<bool, ValueFile> SaveFile(string? title, string? defaultFileName, (string Name, string Type)[]? filters)
    {
        CoCreateInstance<FileSaveDialog, IFileSaveDialog>(default, CLSCTX.CLSCTX_INPROC_SERVER, out IFileSaveDialog dialog).ThrowOnFailure();

        FILEOPENDIALOGOPTIONS options =
            FILEOPENDIALOGOPTIONS.FOS_NOTESTFILECREATE |
            FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM |
            FILEOPENDIALOGOPTIONS.FOS_STRICTFILETYPES |
            FILEOPENDIALOGOPTIONS.FOS_NOCHANGEDIR;

        dialog.SetOptions(options);
        SetDesktopAsStartupFolder(dialog);

        if (!string.IsNullOrEmpty(title))
        {
            dialog.SetTitle(title);
        }

        if (!string.IsNullOrEmpty(defaultFileName))
        {
            dialog.SetFileName(defaultFileName);
        }

        if (filters is { Length: > 0 })
        {
            SetFileTypes(dialog, filters);
        }

        HRESULT res = dialog.Show(currentWindowReference.GetWindowHandle());
        if (res == HRESULT_FROM_WIN32(WIN32_ERROR.ERROR_CANCELLED))
        {
            return new(false, default);
        }
        else
        {
            Marshal.ThrowExceptionForHR(res);
        }

        dialog.GetResult(out IShellItem item);

        PWSTR displayName = default;
        string file;
        try
        {
            item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out displayName);
            file = new((char*)displayName);
        }
        finally
        {
            Marshal.FreeCoTaskMem((nint)displayName.Value);
        }

        return new(true, file);
    }

    public unsafe ValueResult<bool, string> PickFolder(string? title)
    {
        CoCreateInstance<FileOpenDialog, IFileOpenDialog>(default, CLSCTX.CLSCTX_INPROC_SERVER, out IFileOpenDialog dialog).ThrowOnFailure();

        FILEOPENDIALOGOPTIONS options =
            FILEOPENDIALOGOPTIONS.FOS_NOTESTFILECREATE |
            FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM |
            FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS |
            FILEOPENDIALOGOPTIONS.FOS_NOCHANGEDIR;

        dialog.SetOptions(options);
        SetDesktopAsStartupFolder(dialog);

        if (!string.IsNullOrEmpty(title))
        {
            dialog.SetTitle(title);
        }

        HRESULT res = dialog.Show(currentWindowReference.GetWindowHandle());
        if (res == HRESULT_FROM_WIN32(WIN32_ERROR.ERROR_CANCELLED))
        {
            return new(false, default!);
        }
        else
        {
            Marshal.ThrowExceptionForHR(res);
        }

        dialog.GetResult(out IShellItem item);

        PWSTR displayName = default;
        string file;
        try
        {
            item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out displayName);
            file = new((char*)displayName);
        }
        finally
        {
            Marshal.FreeCoTaskMem((nint)displayName.Value);
        }

        return new(true, file);
    }

    private static unsafe void SetFileTypes<TDialog>(TDialog dialog, (string Name, string Type)[] filters)
        where TDialog : IFileDialog
    {
        List<nint> unmanagedStringPtrs = new(filters.Length * 2);
        List<COMDLG_FILTERSPEC> filterSpecs = new(filters.Length);
        foreach ((string name, string type) in filters)
        {
            nint pName = Marshal.StringToHGlobalUni(name);
            nint pType = Marshal.StringToHGlobalUni(type);
            unmanagedStringPtrs.Add(pName);
            unmanagedStringPtrs.Add(pType);
            COMDLG_FILTERSPEC spec = default;
            spec.pszName = *(PCWSTR*)&pName;
            spec.pszSpec = *(PCWSTR*)&pType;
            filterSpecs.Add(spec);
        }

        fixed (COMDLG_FILTERSPEC* ptr = CollectionsMarshal.AsSpan(filterSpecs))
        {
            dialog.SetFileTypes((uint)filterSpecs.Count, ptr);
        }

        foreach (ref readonly nint ptr in CollectionsMarshal.AsSpan(unmanagedStringPtrs))
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    private static unsafe void SetDesktopAsStartupFolder<TDialog>(TDialog dialog)
        where TDialog : IFileDialog
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        SHCreateItemFromParsingName(desktopPath, default, typeof(IShellItem).GUID, out object shellItem).ThrowOnFailure();
        dialog.SetFolder((IShellItem)shellItem);
    }
}