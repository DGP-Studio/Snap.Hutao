// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Windows.Win32;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao.Core.Shell;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IShellLinkInterop))]
internal sealed partial class ShellLinkInterop : IShellLinkInterop
{
    private readonly RuntimeOptions runtimeOptions;

    public void CreateDesktopShoutcutForElevatedLaunch()
    {
        string sourceLogoPath = Path.Combine(runtimeOptions.InstalledLocation, "Assets/Logo.ico");
        string targetLogoPath = Path.Combine(runtimeOptions.DataFolder, "ShellLinkLogo.ico");
        File.Copy(sourceLogoPath, targetLogoPath);

        IShellLinkW shellLink = (IShellLinkW)new ShellLink();
        shellLink.SetPath("powershell");
        shellLink.SetArguments($"""
            -Command "Start-Process shell:AppsFolder\{runtimeOptions.FamilyName}!App -verb runas"
            """);
        shellLink.SetShowCmd(SHOW_WINDOW_CMD.SW_SHOWMINNOACTIVE);

        shellLink.SetIconLocation(targetLogoPath, 0);
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string target = Path.Combine(desktop, $"{SH.AppNameAndVersion.Format(runtimeOptions.Version)}.lnk");

        IPersistFile persistFile = (IPersistFile)shellLink;
        persistFile.Save(target, false);
    }
}