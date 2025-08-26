// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using System.IO;

namespace Snap.Hutao.Core.Shell;

[Service(ServiceLifetime.Transient, typeof(IShellLinkInterop))]
internal sealed class ShellLinkInterop : IShellLinkInterop
{
    public bool TryCreateDesktopShortcutForElevatedLaunch()
    {
        string targetLogoPath = HutaoRuntime.GetDataDirectoryFile("ShellLinkLogo.ico");
        string elevatedLauncherPath = HutaoRuntime.GetDataDirectoryFile("Snap.Hutao.Elevated.Launcher.exe");

        try
        {
            InstalledLocation.CopyFileFromApplicationUri("ms-appx:///Assets/Logo.ico", targetLogoPath);
            InstalledLocation.CopyFileFromApplicationUri("ms-appx:///Snap.Hutao.Elevated.Launcher.exe", elevatedLauncherPath);

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string target = Path.Combine(desktop, $"{SH.FormatAppNameAndVersion(HutaoRuntime.Version)}.lnk");
            FileSystem.CreateLink(elevatedLauncherPath, HutaoRuntime.FamilyName, targetLogoPath, target);
            return true;
        }
        catch
        {
            return false;
        }
    }
}