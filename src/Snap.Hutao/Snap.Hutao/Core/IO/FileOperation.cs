// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class FileOperation
{
    public static void Copy(ValueFile source, ValueFile target, bool overwrite)
    {
        try
        {
            File.Copy(source, target, overwrite);
        }
        catch (IOException ex)
        {
            if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_ENCRYPTION_FAILED))
            {
                try
                {
                    FileSystem.CopyFileAllowDecryptedDestination(Path.GetFullPath(source), Path.GetFullPath(target), overwrite);
                }
                catch (Exception)
                {
                    using (FileStream srcStream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (FileStream destStream = new(target, overwrite ? FileMode.Create : FileMode.CreateNew))
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

    public static bool Move(ValueFile source, ValueFile target, bool overwrite)
    {
        if (!File.Exists(source))
        {
            return false;
        }

        if (overwrite)
        {
            try
            {
                File.Move(source, target, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        if (File.Exists(target))
        {
            return false;
        }

        try
        {
            File.Move(source, target, false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool Delete(ValueFile file)
    {
        if (!File.Exists(file))
        {
            return false;
        }

        File.Delete(file);
        return true;
    }

    public static void UnsafeDelete(ValueFile file)
    {
        FileSystem.DeleteItem(file);
    }

    public static void UnsafeMove(ValueFile source, ValueFile target)
    {
        string? targetDirectory = Path.GetDirectoryName(target);
        ArgumentException.ThrowIfNullOrEmpty(targetDirectory);
        string fileName = Path.GetFileName(target);
        ArgumentException.ThrowIfNullOrEmpty(fileName);
        FileSystem.MoveItem(source, targetDirectory, fileName);
    }
}