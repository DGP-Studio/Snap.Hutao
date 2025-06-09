// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.UI.Shell;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class DirectoryOperation
{
    public static long GetSize(string path, CancellationToken token = default)
    {
        if (!Directory.Exists(path))
        {
            return 0;
        }

        long size = 0;
        try
        {
            foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    size += new FileInfo(file).Length;
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
        catch (Exception)
        {
            return 0;
        }

        return size;
    }

    public static bool Copy(string sourceDirName, string destDirName, out Exception? exception)
    {
        if (!Directory.Exists(sourceDirName))
        {
            exception = new DirectoryNotFoundException($"Directory not found: {sourceDirName}");
            return false;
        }

        try
        {
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(sourceDirName, destDirName, true);
            exception = default;
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }

    public static bool Move(string sourceDirName, string destDirName)
    {
        if (!Directory.Exists(sourceDirName))
        {
            return false;
        }

        try
        {
            Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(sourceDirName, destDirName, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void UnsafeRename(string path, string name, FILEOPERATION_FLAGS flags = FILEOPERATION_FLAGS.FOF_ALLOWUNDO | FILEOPERATION_FLAGS.FOF_NOCONFIRMMKDIR)
    {
        FileSystem.RenameItem(path, name, flags);
    }
}