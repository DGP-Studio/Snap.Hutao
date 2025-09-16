// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Notification;
using System.IO;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingGameViewModel : Abstraction.ViewModel
{
    private readonly IInfoBarService infoBarService;
    private readonly LaunchOptions launchOptions;

    public partial AppOptions AppOptions { get; }

    public int KiloBytesPerSecondLimit
    {
        get => AppOptions.DownloadSpeedLimitPerSecondInKiloByte.Value;
        set => AppOptions.DownloadSpeedLimitPerSecondInKiloByte.Value = value;
    }

    [Command("DeleteGameWebCacheCommand")]
    private void DeleteGameWebCache()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Delete game web cache", "SettingGameViewModel.Command"));

        string? gamePath = launchOptions.GamePathEntry.Value?.Path;

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