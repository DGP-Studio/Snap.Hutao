// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualBasic.FileIO;
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

        FileSystem.MoveDirectory(sourceDirName, destDirName, true);
        return true;
    }
}
