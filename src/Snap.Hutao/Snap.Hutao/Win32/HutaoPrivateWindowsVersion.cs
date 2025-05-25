// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32;

internal readonly struct HutaoPrivateWindowsVersion
{
    public readonly uint Major;
    public readonly uint Minor;
    public readonly uint Build;
    public readonly uint Revision;

    public static implicit operator Version(HutaoPrivateWindowsVersion version)
    {
        return new((int)version.Major, (int)version.Minor, (int)version.Build, (int)version.Revision);
    }
}