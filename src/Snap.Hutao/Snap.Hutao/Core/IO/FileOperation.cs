// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class FileOperation
{
    public static void Copy(string source, string destination, bool overwrite)
    {
        try
        {
            File.Copy(source, destination, overwrite);
        }
        catch (IOException ex)
        {
            if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_ENCRYPTION_FAILED))
            {
                try
                {
                    FileSystem.CopyFileAllowDecryptedDestination(Path.GetFullPath(source), Path.GetFullPath(destination), overwrite);
                }
                catch (Exception)
                {
                    using (FileStream srcStream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (FileStream destStream = new(destination, overwrite ? FileMode.Create : FileMode.CreateNew))
                        {
                            srcStream.CopyTo(destStream);
                        }
                    }
                }
            }
            else
            {
                throw;
            }
        }
    }

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

    public static void UnsafeDelete(string path)
    {
        FileSystem.DeleteItem(path);
    }

    public static void UnsafeMove(string sourceFileName, string destFileName)
    {
        string? destFolder = Path.GetDirectoryName(destFileName);
        ArgumentException.ThrowIfNullOrEmpty(destFolder);
        string fileName = Path.GetFileName(destFileName);
        ArgumentException.ThrowIfNullOrEmpty(fileName);
        FileSystem.MoveItem(sourceFileName, destFolder, fileName);
    }
}