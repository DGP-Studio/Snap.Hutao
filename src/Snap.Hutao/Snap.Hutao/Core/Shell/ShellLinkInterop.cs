// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.IO;
using Windows.Storage;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.Ole32;

namespace Snap.Hutao.Core.Shell;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IShellLinkInterop))]
internal sealed partial class ShellLinkInterop : IShellLinkInterop
{
    private readonly RuntimeOptions runtimeOptions;

    public async ValueTask<bool> TryCreateDesktopShoutcutForElevatedLaunchAsync()
    {
        Uri elevatedLauncherUri = "ms-appx:///Snap.Hutao.Elevated.Launcher.exe".ToUri();
        StorageFile launcherFile = await StorageFile.GetFileFromApplicationUriAsync(elevatedLauncherUri);
        string elevatedLauncherPath = launcherFile.Path;

        return UnsafeTryCreateDesktopShoutcutForElevatedLaunch(elevatedLauncherPath);
    }

    private unsafe bool UnsafeTryCreateDesktopShoutcutForElevatedLaunch(string elevatedLauncherPath)
    {
        bool result = false;

        // DO NOT revert if condition, COM interfaces need to be released properly
        HRESULT hr = CoCreateInstance(in ShellLink.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IShellLinkW.IID, out IShellLinkW* pShellLink);
        if (SUCCEEDED(hr))
        {
            pShellLink->SetPath(elevatedLauncherPath);
            pShellLink->SetArguments(runtimeOptions.FamilyName);
            pShellLink->SetShowCmd(SHOW_WINDOW_CMD.SW_NORMAL);

            if (SUCCEEDED(pShellLink->QueryInterface(in IPersistFile.IID, out IPersistFile* pPersistFile)))
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string target = Path.Combine(desktop, $"{SH.FormatAppNameAndVersion(runtimeOptions.Version)}.lnk");

                if (SUCCEEDED(pPersistFile->Save(target, false)))
                {
                    result = true;
                }

                pPersistFile->Release();
            }

            pShellLink->Release();
        }

        return result;
    }
}