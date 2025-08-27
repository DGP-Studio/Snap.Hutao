// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class LogicalDrive
{
    public static long GetAvailableFreeSpace(ValueDirectory directory)
    {
        if (!directory.HasValue)
        {
            return 0;
        }

        string path = directory;
        if (!path.EndsWith('\\'))
        {
            path += '\\';
        }

        if (Uri.TryCreate(path, UriKind.Absolute, out Uri? pathUri) && pathUri.IsUnc)
        {
            return HutaoNative.Instance.MakeLogicalDrive().GetDiskFreeSpace(path);
        }

        try
        {
            string? root = Path.GetPathRoot(path);
            ArgumentException.ThrowIfNullOrWhiteSpace(root, "The path does not contain a root.");
            return new DriveInfo(root).AvailableFreeSpace;
        }
        catch (ArgumentException)
        {
            return HutaoNative.Instance.MakeLogicalDrive().GetDiskFreeSpace(path);
        }
    }
}