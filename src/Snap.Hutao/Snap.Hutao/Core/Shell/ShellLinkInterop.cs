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
        string targetLogoPath = Path.Combine(runtimeOptions.DataFolder, "ShellLinkLogo.ico");

        try
        {
            Uri sourceLogoUri = "ms-appx:///Assets/Logo.ico".ToUri();
            StorageFile iconFile = await StorageFile.GetFileFromApplicationUriAsync(sourceLogoUri);
            await iconFile.OverwriteCopyAsync(targetLogoPath).ConfigureAwait(false);
        }
        catch
        {
            return false;
        }

        return UnsafeTryCreateDesktopShoutcutForElevatedLaunch(targetLogoPath);
    }

    private unsafe bool UnsafeTryCreateDesktopShoutcutForElevatedLaunch(string targetLogoPath)
    {
        bool result = false;

        // DO NOT revert if condition, COM interfaces need to be released properly
        HRESULT hr = CoCreateInstance(in ShellLink.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IShellLinkW.IID, out IShellLinkW* pShellLink);
        if (SUCCEEDED(hr))
        {
            pShellLink->SetPath($"shell:AppsFolder\\{runtimeOptions.FamilyName}!App");
            pShellLink->SetShowCmd(SHOW_WINDOW_CMD.SW_NORMAL);
            pShellLink->SetIconLocation(targetLogoPath, 0);

            if (SUCCEEDED(pShellLink->QueryInterface(in IShellLinkDataList.IID, out IShellLinkDataList* pShellLinkDataList)))
            {
                pShellLinkDataList->GetFlags(out uint flags);
                pShellLinkDataList->SetFlags(flags | (uint)SHELL_LINK_DATA_FLAGS.SLDF_RUNAS_USER);
                pShellLinkDataList->Release();
            }

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