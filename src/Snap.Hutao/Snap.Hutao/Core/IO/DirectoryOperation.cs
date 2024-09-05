// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualBasic.FileIO;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell;
using System.IO;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.Ole32;
using static Snap.Hutao.Win32.Shell32;

namespace Snap.Hutao.Core.IO;

internal static class DirectoryOperation
{
    public static bool Move(string sourceDirName, string destDirName)
    {
        if (!Directory.Exists(sourceDirName))
        {
            return false;
        }

        try
        {
            FileSystem.MoveDirectory(sourceDirName, destDirName, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static unsafe bool UnsafeRename(string path, string name, FILEOPERATION_FLAGS flags = FILEOPERATION_FLAGS.FOF_ALLOWUNDO | FILEOPERATION_FLAGS.FOF_NOCONFIRMMKDIR)
    {
        bool result = false;

        if (SUCCEEDED(CoCreateInstance(in Win32.UI.Shell.FileOperation.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IFileOperation.IID, out IFileOperation* pFileOperation)))
        {
            if (SUCCEEDED(SHCreateItemFromParsingName(path, default, in IShellItem.IID, out IShellItem* pShellItem)))
            {
                pFileOperation->SetOperationFlags(flags);
                pFileOperation->RenameItem(pShellItem, name, default);

                if (SUCCEEDED(pFileOperation->PerformOperations()))
                {
                    result = true;
                }

                IUnknownMarshal.Release(pShellItem);
            }

            IUnknownMarshal.Release(pFileOperation);
        }

        return result;
    }
}
