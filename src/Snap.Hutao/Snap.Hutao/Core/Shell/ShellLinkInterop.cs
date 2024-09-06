// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.IO;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.Ole32;

namespace Snap.Hutao.Core.Shell;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IShellLinkInterop))]
internal sealed partial class ShellLinkInterop : IShellLinkInterop
{
    private readonly RuntimeOptions runtimeOptions;

    public ValueTask<bool> TryCreateDesktopShoutcutForElevatedLaunchAsync()
    {
        string targetLogoPath = Path.Combine(runtimeOptions.DataFolder, "ShellLinkLogo.ico");
        string elevatedLauncherPath = Path.Combine(runtimeOptions.DataFolder, "Snap.Hutao.Elevated.Launcher.exe");

        try
        {
            InstalledLocation.CopyFileFromApplicationUri("ms-appx:///Assets/Logo.ico", targetLogoPath);
            InstalledLocation.CopyFileFromApplicationUri("ms-appx:///Snap.Hutao.Elevated.Launcher.exe", elevatedLauncherPath);
        }
        catch
        {
            return ValueTask.FromResult(false);
        }

        bool result = UnsafeTryCreateDesktopShoutcutForElevatedLaunch(targetLogoPath, elevatedLauncherPath);
        return ValueTask.FromResult(result);
    }

    private unsafe bool UnsafeTryCreateDesktopShoutcutForElevatedLaunch(string targetLogoPath, string elevatedLauncherPath)
    {
        bool result = false;

        // DO NOT revert if condition, COM interfaces need to be released properly
        HRESULT hr = CoCreateInstance(in ShellLink.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IShellLinkW.IID, out IShellLinkW* pShellLink);
        if (SUCCEEDED(hr))
        {
            pShellLink->SetPath(elevatedLauncherPath);
            pShellLink->SetArguments(runtimeOptions.FamilyName);
            pShellLink->SetShowCmd(SHOW_WINDOW_CMD.SW_NORMAL);
            pShellLink->SetIconLocation(targetLogoPath, 0);

            if (SUCCEEDED(IUnknownMarshal.QueryInterface(pShellLink, in IPersistFile.IID, out IPersistFile* pPersistFile)))
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string target = Path.Combine(desktop, $"{SH.FormatAppNameAndVersion(runtimeOptions.Version)}.lnk");

                if (SUCCEEDED(pPersistFile->Save(target, false)))
                {
                    result = true;
                }

                IUnknownMarshal.Release(pPersistFile);
            }

            IUnknownMarshal.Release(pShellLink);
        }

        return result;
    }
}