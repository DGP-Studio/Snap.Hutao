// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell;
using System.IO;
using WinRT;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.Ole32;
using static Snap.Hutao.Win32.Shell32;

namespace Snap.Hutao.Core.IO;

internal static class FileOperation
{
    /// <summary>
    /// 将指定文件移动到新位置，提供指定新文件名和覆盖目标文件（如果它已存在）的选项。
    /// </summary>
    /// <param name="sourceFileName">要移动的文件的名称。 可以包括相对或绝对路径。</param>
    /// <param name="destFileName">文件的新路径和名称。</param>
    /// <param name="overwrite">如果要覆盖目标文件</param>
    /// <returns>是否发生了移动操作</returns>
    public static bool Move(string sourceFileName, string destFileName, bool overwrite)
    {
        if (!File.Exists(sourceFileName))
        {
            return false;
        }

        if (overwrite)
        {
            try
            {
                File.Move(sourceFileName, destFileName, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        if (File.Exists(destFileName))
        {
            return false;
        }

        try
        {
            File.Move(sourceFileName, destFileName, false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool Delete(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        File.Delete(path);
        return true;
    }

    public static unsafe bool UnsafeDelete(string path)
    {
        if (!SUCCEEDED(CoCreateInstance(in Win32.UI.Shell.FileOperation.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IFileOperation.IID, out ObjectReference<IFileOperation.Vftbl> fileOperation)))
        {
            return false;
        }

        using (fileOperation)
        {
            if (!SUCCEEDED(SHCreateItemFromParsingName(path, default, in IShellItem.IID, out ObjectReference<IShellItem.Vftbl> shellItem)))
            {
                return false;
            }

            using (shellItem)
            {
                fileOperation.DeleteItem(shellItem, default!);
                return SUCCEEDED(fileOperation.PerformOperations());
            }
        }
    }

    public static unsafe bool UnsafeMove(string sourceFileName, string destFileName)
    {
        if (!SUCCEEDED(CoCreateInstance(in Win32.UI.Shell.FileOperation.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IFileOperation.IID, out ObjectReference<IFileOperation.Vftbl> fileOperation)))
        {
            return false;
        }

        using (fileOperation)
        {
            if (!SUCCEEDED(SHCreateItemFromParsingName(sourceFileName, default, in IShellItem.IID, out ObjectReference<IShellItem.Vftbl> sourceShellItem)))
            {
                return false;
            }

            using (sourceShellItem)
            {
                if (!SUCCEEDED(SHCreateItemFromParsingName(destFileName, default, in IShellItem.IID, out ObjectReference<IShellItem.Vftbl> destShellItem)))
                {
                    return false;
                }

                using (destShellItem)
                {
                    fileOperation.MoveItem(sourceShellItem, destShellItem, default, default!);
                    return SUCCEEDED(fileOperation.PerformOperations());
                }
            }
        }
    }
}