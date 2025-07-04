// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.UI.Shell;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

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

    public static bool TrySetFullControl(string path)
    {
        if (!Directory.Exists(path))
        {
            return false;
        }

        if (WindowsIdentity.GetCurrent().User is not { } currentUser)
        {
            return false;
        }

        try
        {
            DirectoryInfo info = new(path);
            DirectorySecurity accessControl = info.GetAccessControl();

            // Once we get access rules, it's cached, so we can safely enumerate and remove rules at the same time.
            foreach (FileSystemAccessRule rule in accessControl.GetAccessRules(true, true, typeof(SecurityIdentifier)))
            {
                if (rule.IdentityReference == currentUser && rule.AccessControlType == AccessControlType.Deny)
                {
                    accessControl.RemoveAccessRule(rule);
                }
            }

            FileSystemAccessRule accessRule = new(
                currentUser,
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.InheritOnly,
                AccessControlType.Allow);
            accessControl.RemoveAccessRuleAll(accessRule);
            accessControl.AddAccessRule(accessRule);
            info.SetAccessControl(accessControl);

            return true;
        }
        catch
        {
            return false;
        }
    }
}