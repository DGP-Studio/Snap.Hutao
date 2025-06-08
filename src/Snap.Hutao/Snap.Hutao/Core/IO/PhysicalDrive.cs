// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Win32;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class PhysicalDrive
{
    /// <summary>
    /// Safely get the SSD information of the physical driver.
    /// </summary>
    /// <param name="path">path in a driver</param>
    /// <returns>
    /// <see langword="null"/> if any exception occurs,
    /// <see langword="true"/> if it's a SSD,
    /// otherwise <see langword="false"/>
    /// </returns>
    public static bool? GetIsSolidState(string path)
    {
        if (Uri.TryCreate(path, UriKind.Absolute, out Uri? pathUri))
        {
            if (pathUri.IsUnc)
            {
                return false;
            }
        }

        try
        {
            return DangerousGetIsSolidState(path);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static bool DangerousGetIsSolidState(string path)
    {
        if (LocalSetting.Get(SettingKeys.OverridePhysicalDriverType, false))
        {
            return LocalSetting.Get(SettingKeys.PhysicalDriverIsAlwaysSolidState, false);
        }

        string? root = Path.GetPathRoot(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(root, "The path does not contain a root.");
        return HutaoNative.Instance.MakePhysicalDrive().IsPathOnSolidStateDrive(root);
    }
}