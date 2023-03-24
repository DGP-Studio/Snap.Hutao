// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.View.Dialog;
using Windows.Storage.Pickers;
using EntityAchievementArchive = Snap.Hutao.Model.Entity.AchievementArchive;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 成就导入器
/// </summary>
[HighQuality]
internal sealed class AchievementImporter
{
    private readonly IServiceProvider serviceProvider;
    private readonly IAchievementService achievementService;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;

    /// <summary>
    /// 构造一个新的成就导入器
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public AchievementImporter(IServiceProvider serviceProvider)
    {
        achievementService = serviceProvider.GetRequiredService<IAchievementService>();
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        options = serviceProvider.GetRequiredService<JsonSerializerOptions>();

        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 从剪贴板导入
    /// </summary>
    /// <returns>是否导入成功</returns>
    public async Task<bool> FromClipboardAsync()
    {
        if (achievementService.CurrentArchive != null)
        {
            if (await GetUIAFFromClipboardAsync().ConfigureAwait(false) is UIAF uiaf)
            {
                return await TryImportAsync(achievementService.CurrentArchive!, uiaf).ConfigureAwait(false);
            }
            else
            {
                infoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage);
            }
        }
        else
        {
            infoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2);
        }

        return false;
    }

    /// <summary>
    /// 从文件导入
    /// </summary>
    /// <returns>是否导入成功</returns>
    public async Task<bool> FromFileAsync()
    {
        if (achievementService.CurrentArchive != null)
        {
            (bool isPickerOk, FilePath file) = await serviceProvider
                .GetRequiredService<IPickerFactory>()
                .GetFileOpenPicker(PickerLocationId.Desktop, SH.FilePickerImportCommit, ".json")
                .TryPickSingleFileAsync()
                .ConfigureAwait(false);

            if (isPickerOk)
            {
                (bool isOk, UIAF? uiaf) = await file.DeserializeFromJsonAsync<UIAF>(options).ConfigureAwait(false);

                if (isOk)
                {
                    return await TryImportAsync(achievementService.CurrentArchive, uiaf!).ConfigureAwait(false);
                }
                else
                {
                    infoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage);
                }
            }
        }
        else
        {
            infoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2);
        }

        return false;
    }

    private async Task<UIAF?> GetUIAFFromClipboardAsync()
    {
        try
        {
            return await Clipboard.DeserializeTextAsync<UIAF>(options).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            infoBarService?.Error(ex, SH.ViewModelImportFromClipboardErrorTitle);
            return null;
        }
    }

    private async Task<bool> TryImportAsync(EntityAchievementArchive archive, UIAF uiaf)
    {
        if (uiaf.IsCurrentVersionSupported())
        {
            // ContentDialog must be created by main thread.
            await ThreadHelper.SwitchToMainThreadAsync();
            (bool isOk, ImportStrategy strategy) = await new AchievementImportDialog(uiaf).GetImportStrategyAsync().ConfigureAwait(true);

            if (isOk)
            {
                ImportResult result;
                ContentDialog dialog = await serviceProvider.GetRequiredService<IContentDialogFactory>()
                    .CreateForIndeterminateProgressAsync(SH.ViewModelAchievementImportProgress)
                    .ConfigureAwait(false);

                using (await dialog.BlockAsync().ConfigureAwait(false))
                {
                    result = await achievementService.ImportFromUIAFAsync(archive, uiaf.List, strategy).ConfigureAwait(false);
                }

                infoBarService.Success(result.ToString());
                return true;
            }
        }
        else
        {
            infoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelAchievementImportWarningMessage);
        }

        return false;
    }
}