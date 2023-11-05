// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.View.Dialog;
using Windows.Storage.Pickers;
using EntityAchievementArchive = Snap.Hutao.Model.Entity.AchievementArchive;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 成就导入器
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementImporter
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IAchievementService achievementService;
    private readonly IClipboardInterop clipboardInterop;
    private readonly IInfoBarService infoBarService;
    private readonly IPickerFactory pickerFactory;
    private readonly JsonSerializerOptions options;
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 从剪贴板导入
    /// </summary>
    /// <returns>是否导入成功</returns>
    public async ValueTask<bool> FromClipboardAsync()
    {
        if (achievementService.CurrentArchive is { } archive)
        {
            if (await TryCatchGetUIAFFromClipboardAsync().ConfigureAwait(false) is { } uiaf)
            {
                return await TryImportAsync(archive, uiaf).ConfigureAwait(false);
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
    public async ValueTask<bool> FromFileAsync()
    {
        if (achievementService.CurrentArchive is { } archive)
        {
            ValueResult<bool, ValueFile> pickerResult = await pickerFactory
                .GetFileOpenPicker(PickerLocationId.Desktop, SH.FilePickerImportCommit, ".json")
                .TryPickSingleFileAsync()
                .ConfigureAwait(false);

            if (pickerResult.TryGetValue(out ValueFile file))
            {
                ValueResult<bool, UIAF?> uiafResult = await file.DeserializeFromJsonAsync<UIAF>(options).ConfigureAwait(false);

                if (uiafResult.TryGetValue(out UIAF? uiaf))
                {
                    return await TryImportAsync(archive, uiaf).ConfigureAwait(false);
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

    private async ValueTask<UIAF?> TryCatchGetUIAFFromClipboardAsync()
    {
        try
        {
            return await clipboardInterop.DeserializeFromJsonAsync<UIAF>().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            infoBarService?.Error(ex, SH.ViewModelImportFromClipboardErrorTitle);
            return null;
        }
    }

    private async ValueTask<bool> TryImportAsync(EntityAchievementArchive archive, UIAF uiaf)
    {
        if (uiaf.IsCurrentVersionSupported())
        {
            AchievementImportDialog importDialog = await contentDialogFactory.CreateInstanceAsync<AchievementImportDialog>(uiaf).ConfigureAwait(false);
            (bool isOk, ImportStrategy strategy) = await importDialog.GetImportStrategyAsync().ConfigureAwait(false);

            if (isOk)
            {
                await taskContext.SwitchToMainThreadAsync();
                ContentDialog dialog = await contentDialogFactory
                    .CreateForIndeterminateProgressAsync(SH.ViewModelAchievementImportProgress)
                    .ConfigureAwait(false);

                ImportResult result;
                using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
                {
                    result = await achievementService.ImportFromUIAFAsync(archive, uiaf.List, strategy).ConfigureAwait(false);
                }

                infoBarService.Success($"{result}");
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