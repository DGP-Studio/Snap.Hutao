// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class DirectoryOperation
{
    public static bool Move(string sourceDirName, string destDirName)
    {
        if (!Directory.Exists(sourceDirName))
        {
            return false;
        }

        Directory.Move(sourceDirName, destDirName);
        return true;
    }
}