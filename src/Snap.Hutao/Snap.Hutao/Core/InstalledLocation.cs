// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Windows.ApplicationModel;

namespace Snap.Hutao.Core;

internal static class InstalledLocation
{
    public static string GetAbsolutePath(string relativePath)
    {
        return Path.Combine(Package.Current.InstalledLocation.Path, relativePath);
    }
}