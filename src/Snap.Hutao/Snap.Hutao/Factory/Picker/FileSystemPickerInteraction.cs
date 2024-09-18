// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell;
using Snap.Hutao.Win32.UI.Shell.Common;
using System.Runtime.InteropServices;
using WinRT;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.Ole32;
using static Snap.Hutao.Win32.Shell32;

namespace Snap.Hutao.Factory.Picker;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IFileSystemPickerInteraction))]
internal sealed partial class FileSystemPickerInteraction : IFileSystemPickerInteraction
{
    private readonly ICurrentXamlWindowReference currentWindowReference;

    public unsafe ValueResult<bool, ValueFile> PickFile(string? title, string? defaultFileName, (string Name, string Type)[]? filters)
    {
        HRESULT hr = CoCreateInstance(in FileOpenDialog.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IFileDialog.IID, out ObjectReference<IFileDialog.Vftbl> fileDialog);
        Marshal.ThrowExceptionForHR(hr);

        using (fileDialog)
        {
            IFileDialog* pFileDialog = (IFileDialog*)fileDialog.ThisPtr;

            FILEOPENDIALOGOPTIONS options =
            FILEOPENDIALOGOPTIONS.FOS_NOTESTFILECREATE |
            FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM |
            FILEOPENDIALOGOPTIONS.FOS_NOCHANGEDIR;

            pFileDialog->SetOptions(options);
            SetDesktopAsStartupFolder(pFileDialog);

            if (!string.IsNullOrEmpty(defaultFileName))
            {
                pFileDialog->SetFileName(defaultFileName);
            }

            if (!string.IsNullOrEmpty(title))
            {
                pFileDialog->SetTitle(title);
            }

            if (filters is { Length: > 0 })
            {
                SetFileTypes(pFileDialog, filters);
            }

            HRESULT res = pFileDialog->Show(currentWindowReference.GetWindowHandle());
            if (res == HRESULT_FROM_WIN32(WIN32_ERROR.ERROR_CANCELLED))
            {
                return new(false, default);
            }
            else
            {
                Marshal.ThrowExceptionForHR(res);
            }

            HRESULT t = pFileDialog->GetResult(out ObjectReference<IShellItem.Vftbl> shellItem);

            using (shellItem)
            {
                IShellItem* pShellItem = (IShellItem*)shellItem.ThisPtr;

                PWSTR displayName = default;
                string file;
                try
                {
                    pShellItem->GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out displayName);
                    file = new(displayName);
                }
                finally
                {
                    CoTaskMemFree(displayName);
                }

                return new(true, file);
            }
        }
    }

    public unsafe ValueResult<bool, ValueFile> SaveFile(string? title, string? defaultFileName, (string Name, string Type)[]? filters)
    {
        HRESULT hr = CoCreateInstance(in FileSaveDialog.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IFileDialog.IID, out ObjectReference<IFileDialog.Vftbl> fileDialog);
        Marshal.ThrowExceptionForHR(hr);

        using (fileDialog)
        {
            IFileDialog* pFileDialog = (IFileDialog*)fileDialog.ThisPtr;

            FILEOPENDIALOGOPTIONS options =
                FILEOPENDIALOGOPTIONS.FOS_OVERWRITEPROMPT |
                FILEOPENDIALOGOPTIONS.FOS_NOTESTFILECREATE |
                FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM |
                FILEOPENDIALOGOPTIONS.FOS_STRICTFILETYPES |
                FILEOPENDIALOGOPTIONS.FOS_NOCHANGEDIR;

            pFileDialog->SetOptions(options);
            SetDesktopAsStartupFolder(pFileDialog);

            if (!string.IsNullOrEmpty(defaultFileName))
            {
                pFileDialog->SetFileName(defaultFileName);
            }

            if (!string.IsNullOrEmpty(title))
            {
                pFileDialog->SetTitle(title);
            }

            if (filters is { Length: > 0 })
            {
                SetFileTypes(pFileDialog, filters);
            }

            HRESULT res = pFileDialog->Show(currentWindowReference.GetWindowHandle());
            if (res == HRESULT_FROM_WIN32(WIN32_ERROR.ERROR_CANCELLED))
            {
                return new(false, default);
            }
            else
            {
                Marshal.ThrowExceptionForHR(res);
            }

            pFileDialog->GetResult(out ObjectReference<IShellItem.Vftbl> shellItem);

            using (shellItem)
            {
                IShellItem* pShellItem = (IShellItem*)shellItem.ThisPtr;

                PWSTR displayName = default;
                string file;
                try
                {
                    pShellItem->GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out displayName);
                    file = new(displayName);
                }
                finally
                {
                    CoTaskMemFree(displayName);
                }

                return new(true, file);
            }
        }
    }

    public unsafe ValueResult<bool, string> PickFolder(string? title)
    {
        HRESULT hr = CoCreateInstance(in FileOpenDialog.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IFileDialog.IID, out ObjectReference<IFileDialog.Vftbl> fileDialog);
        Marshal.ThrowExceptionForHR(hr);

        using (fileDialog)
        {
            IFileDialog* pFileDialog = (IFileDialog*)fileDialog.ThisPtr;

            FILEOPENDIALOGOPTIONS options =
            FILEOPENDIALOGOPTIONS.FOS_NOTESTFILECREATE |
            FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM |
            FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS |
            FILEOPENDIALOGOPTIONS.FOS_NOCHANGEDIR;

            pFileDialog->SetOptions(options);
            SetDesktopAsStartupFolder(pFileDialog);

            if (!string.IsNullOrEmpty(title))
            {
                pFileDialog->SetTitle(title);
            }

            HRESULT res = pFileDialog->Show(currentWindowReference.GetWindowHandle());
            if (res == HRESULT_FROM_WIN32(WIN32_ERROR.ERROR_CANCELLED))
            {
                return new(false, default!);
            }
            else
            {
                Marshal.ThrowExceptionForHR(res);
            }

            pFileDialog->GetResult(out ObjectReference<IShellItem.Vftbl> shellItem);

            using (shellItem)
            {
                IShellItem* pShellItem = (IShellItem*)shellItem.ThisPtr;

                PWSTR displayName = default;
                string file;
                try
                {
                    pShellItem->GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out displayName);
                    file = new((char*)displayName);
                }
                finally
                {
                    CoTaskMemFree(displayName);
                }

                return new(true, file);
            }
        }
    }

    private static unsafe void SetFileTypes(IFileDialog* pFileDialog, (string Name, string Type)[] filters)
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

        pFileDialog->SetFileTypes(CollectionsMarshal.AsSpan(filterSpecs));

        foreach (ref readonly nint ptr in CollectionsMarshal.AsSpan(unmanagedStringPtrs))
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    private static unsafe void SetDesktopAsStartupFolder(IFileDialog* pFileDialog)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        HRESULT hr = SHCreateItemFromParsingName(desktopPath, default, in IShellItem.IID, out ObjectReference<IShellItem.Vftbl> shellItem);
        Marshal.ThrowExceptionForHR(hr);

        using (shellItem)
        {
            pFileDialog->SetFolder(shellItem);
        }
    }
}