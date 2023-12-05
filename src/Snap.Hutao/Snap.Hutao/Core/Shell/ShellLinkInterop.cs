// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Storage;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Shell;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IShellLinkInterop))]
internal sealed partial class ShellLinkInterop : IShellLinkInterop
{
    private readonly RuntimeOptions runtimeOptions;
    private readonly AppOptions appOptions;

    public async ValueTask<bool> TryCreateDesktopShoutcutForElevatedLaunchAsync()
    {
        Uri sourceLogoUri = "ms-appx:///Assets/Logo.ico".ToUri();
        string targetLogoPath = Path.Combine(runtimeOptions.DataFolder, "ShellLinkLogo.ico");

        try
        {
            StorageFile iconFile = await StorageFile.GetFileFromApplicationUriAsync(sourceLogoUri);
            using (Stream inputStream = (await iconFile.OpenReadAsync()).AsStream())
            {
                using (FileStream outputStream = File.Create(targetLogoPath))
                {
                    await inputStream.CopyToAsync(outputStream).ConfigureAwait(false);
                }
            }
        }
        catch
        {
            return false;
        }

        HRESULT result = CoCreateInstance<ShellLink, IShellLinkW>(null, CLSCTX.CLSCTX_INPROC_SERVER, out IShellLinkW shellLink);
        Marshal.ThrowExceptionForHR(result);

        shellLink.SetPath(appOptions.PowerShellPath);
        shellLink.SetArguments($"""
            -Command "Start-Process shell:AppsFolder\{runtimeOptions.FamilyName}!App -verb runas"
            """);
        shellLink.SetShowCmd(SHOW_WINDOW_CMD.SW_SHOWMINNOACTIVE);
        shellLink.SetIconLocation(targetLogoPath, 0);
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string target = Path.Combine(desktop, $"{SH.FormatAppNameAndVersion(runtimeOptions.Version)}.lnk");

        IPersistFile persistFile = (IPersistFile)shellLink;
        try
        {
            persistFile.Save(target, false);
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }

        return true;
    }
}