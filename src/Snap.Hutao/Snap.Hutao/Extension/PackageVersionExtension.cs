// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Windows.ApplicationModel;

namespace Snap.Hutao.Extension;

/// <summary>
/// 包版本扩展
/// </summary>
[HighQuality]
internal static class PackageVersionExtension
{
    /// <summary>
    /// 将包版本转换为版本
    /// </summary>
    /// <param name="packageVersion">包版本</param>
    /// <returns>版本</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Version ToVersion(this PackageVersion packageVersion)
    {
        return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
    }
}