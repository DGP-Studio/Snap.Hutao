// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32;

internal readonly struct HutaoPrivateWindowsVersion
{
    public readonly uint Major;
    public readonly uint Minor;
    public readonly uint Build;
    public readonly uint Revision;

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Build}.{Revision}";
    }
}