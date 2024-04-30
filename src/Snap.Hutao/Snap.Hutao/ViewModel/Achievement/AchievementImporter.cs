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
using EntityAchievementArchive = Snap.Hutao.Model.Entity.AchievementArchive;

namespace Snap.Hutao.ViewModel.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementImporter
{
    private readonly AchievementImporterDependencies dependencies;

    public async ValueTask<bool> FromClipboardAsync()
    {
        if (dependencies.AchievementService.CurrentArchive is not { } archive)
        {
            dependencies.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2);
            return false;
        }

        if (await TryCatchGetUIAFFromClipboardAsync().ConfigureAwait(false) is not { } uiaf)
        {
            dependencies.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage);
            return false;
        }

        return await TryImportAsync(archive, uiaf).ConfigureAwait(false);
    }

    public async ValueTask<bool> FromFileAsync()
    {
        if (dependencies.AchievementService.CurrentArchive is not { } archive)
        {
            dependencies.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2);
            return false;
        }

        ValueResult<bool, ValueFile> pickerResult = dependencies.FileSystemPickerInteraction.PickFile(
            SH.ServiceAchievementUIAFImportPickerTitile,
            [(SH.ServiceAchievementUIAFImportPickerFilterText, "*.json")]);

        if (!pickerResult.TryGetValue(out ValueFile file))
        {
            return false;
        }

        ValueResult<bool, UIAF?> uiafResult = await file.DeserializeFromJsonAsync<UIAF>(dependencies.JsonSerializerOptions).ConfigureAwait(false);

        if (!uiafResult.TryGetValue(out UIAF? uiaf))
        {
            dependencies.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage);
            return false;
        }

        return await TryImportAsync(archive, uiaf).ConfigureAwait(false);
    }

    private async ValueTask<UIAF?> TryCatchGetUIAFFromClipboardAsync()
    {
        try
        {
            return await dependencies.ClipboardProvider.DeserializeFromJsonAsync<UIAF>().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            dependencies.InfoBarService.Error(ex, SH.ViewModelImportFromClipboardErrorTitle);
            return null;
        }
    }

    private async ValueTask<bool> TryImportAsync(EntityAchievementArchive archive, UIAF uiaf)
    {
        if (!uiaf.IsCurrentVersionSupported())
        {
            dependencies.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelAchievementImportWarningMessage);
            return false;
        }

        AchievementImportDialog importDialog = await dependencies.ContentDialogFactory
            .CreateInstanceAsync<AchievementImportDialog>(uiaf).ConfigureAwait(false);
        (bool isOk, ImportStrategyKind strategy) = await importDialog.GetImportStrategyAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return false;
        }

        await dependencies.TaskContext.SwitchToMainThreadAsync();
        ContentDialog dialog = await dependencies.ContentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelAchievementImportProgress)
            .ConfigureAwait(false);

        ImportResult result;
        using (await dialog.BlockAsync(dependencies.TaskContext).ConfigureAwait(false))
        {
            result = await dependencies.AchievementService.ImportFromUIAFAsync(archive, uiaf.List, strategy).ConfigureAwait(false);
        }

        dependencies.InfoBarService.Success($"{result}");
        return true;
    }
}