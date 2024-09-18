// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Windows.ApplicationModel;

namespace Snap.Hutao.Extension;

internal static class PackageVersionExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Version ToVersion(this PackageVersion packageVersion)
    {
        return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
    }
}