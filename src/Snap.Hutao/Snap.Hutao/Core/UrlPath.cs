// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core;

// Ported from System.Configuration
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
internal static class UrlPath
{
    public static string? GetDirectoryOrRootName(string path)
    {
        return Path.GetDirectoryName(path) ?? Path.GetPathRoot(path);
    }

    public static bool IsEqualOrSubdirectory(string directory, string subDirectory)
    {
        return IsEqualOrSubdirectoryImpl(Path.GetFullPath(directory), Path.GetFullPath(subDirectory));
    }

    private static bool IsEqualOrSubdirectoryImpl(string directory, string subDirectory)
    {
        if (string.IsNullOrEmpty(directory))
        {
            return true;
        }

        if (string.IsNullOrEmpty(subDirectory))
        {
            return false;
        }

        // Compare up to but not including trailing backslash
        int directoryLength = directory[^1] is '\\' or '/' ? directory.Length - 1 : directory.Length;
        int subDirectoryLength = subDirectory[^1] is '\\' or '/' ? subDirectory.Length - 1 : subDirectory.Length;

        if (subDirectoryLength < directoryLength)
        {
            return false;
        }

        if (string.Compare(directory, 0, subDirectory, 0, directoryLength, StringComparison.OrdinalIgnoreCase) != 0)
        {
            return false;
        }

        // Check subDirectory that character following length of dir is a backslash
        return (subDirectoryLength == directoryLength) || (subDirectory[directoryLength] is '\\' or '/');
    }
}