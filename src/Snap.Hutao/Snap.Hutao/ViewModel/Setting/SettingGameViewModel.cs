// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Notification;
using System.IO;
using Snap.Hutao.Model;
using Snap.Hutao.Service.Game.Package;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingGameViewModel : Abstraction.ViewModel
{
    private readonly IInfoBarService infoBarService;
    private readonly LaunchOptions launchOptions;
    private readonly AppOptions appOptions;

    private NameValue<PackageConverterMode>? selectedPackageConverterMode;

    public AppOptions AppOptions { get => appOptions; }

    public NameValue<PackageConverterMode>? SelectedPackageConverterMode
    {
        get => selectedPackageConverterMode ??= AppOptions.PackageConverterModes.Single(t => t.Value == AppOptions.PackageConverterMode);
        set
        {
            if (SetProperty(ref selectedPackageConverterMode, value) && value is not null)
            {
                AppOptions.PackageConverterMode = value.Value;
            }
        }
    }

    public int KiloBytesPerSecondLimit
    {
        get => appOptions.DownloadSpeedLimitPerSecondInKiloByte;
        set => appOptions.DownloadSpeedLimitPerSecondInKiloByte = value;
    }

    [Command("DeleteGameWebCacheCommand")]
    private void DeleteGameWebCache()
    {
        string gamePath = launchOptions.GamePath;

        if (string.IsNullOrEmpty(gamePath))
        {
            return;
        }

        string cacheFilePath = GachaLogQueryWebCacheProvider.GetCacheFile(gamePath);
        string? cacheFolder = Path.GetDirectoryName(cacheFilePath);

        if (Directory.Exists(cacheFolder))
        {
            try
            {
                Directory.Delete(cacheFolder, true);
            }
            catch (UnauthorizedAccessException)
            {
                infoBarService.Warning(SH.ViewModelSettingClearWebCacheFail);
                return;
            }

            infoBarService.Success(SH.ViewModelSettingClearWebCacheSuccess);
        }
        else
        {
            infoBarService.Warning(SH.FormatViewModelSettingClearWebCachePathInvalid(cacheFolder));
        }
    }
}