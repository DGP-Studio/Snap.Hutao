// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.View.Dialog;
using EntityAchievementArchive = Snap.Hutao.Model.Entity.AchievementArchive;

namespace Snap.Hutao.ViewModel.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementImporter
{
    private readonly AchievementImporterScopeContext scopeContext;

    public async ValueTask<bool> FromClipboardAsync(AchievementViewModelScopeContext context)
    {
        if (context.AchievementService.Archives.CurrentItem is not { } archive)
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2);
            return false;
        }

        if (await TryCatchGetUIAFFromClipboardAsync().ConfigureAwait(false) is not { } uiaf)
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage);
            return false;
        }

        return await TryImportCoreAsync(context, archive, uiaf).ConfigureAwait(false);
    }

    public async ValueTask<bool> FromFileAsync(AchievementViewModelScopeContext context)
    {
        if (context.AchievementService.Archives.CurrentItem is not { } archive)
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2);
            return false;
        }

        ValueResult<bool, ValueFile> pickerResult = scopeContext.FileSystemPickerInteraction.PickFile(
            SH.ServiceAchievementUIAFImportPickerTitile,
            [(SH.ServiceAchievementUIAFImportPickerFilterText, "*.json")]);

        if (!pickerResult.TryGetValue(out ValueFile file))
        {
            return false;
        }

        ValueResult<bool, UIAF?> uiafResult = await file.DeserializeFromJsonAsync<UIAF>(scopeContext.JsonSerializerOptions).ConfigureAwait(false);

        if (!uiafResult.TryGetValue(out UIAF? uiaf))
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage);
            return false;
        }

        return await TryImportCoreAsync(context, archive, uiaf).ConfigureAwait(false);
    }

    private async ValueTask<UIAF?> TryCatchGetUIAFFromClipboardAsync()
    {
        try
        {
            return await scopeContext.ClipboardProvider.DeserializeFromJsonAsync<UIAF>().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            scopeContext.InfoBarService.Error(ex, SH.ViewModelImportFromClipboardErrorTitle);
            return null;
        }
    }

    private async ValueTask<bool> TryImportCoreAsync(AchievementViewModelScopeContext context, EntityAchievementArchive archive, UIAF uiaf)
    {
        if (!uiaf.IsCurrentVersionSupported())
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelAchievementImportWarningMessage);
            return false;
        }

        AchievementImportDialog importDialog = await scopeContext.ContentDialogFactory
            .CreateInstanceAsync<AchievementImportDialog>(uiaf).ConfigureAwait(false);
        (bool isOk, ImportStrategyKind strategy) = await importDialog.GetImportStrategyAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return false;
        }

        await scopeContext.TaskContext.SwitchToMainThreadAsync();
        ContentDialog dialog = await scopeContext.ContentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelAchievementImportProgress)
            .ConfigureAwait(false);

        ImportResult result;
        using (await dialog.BlockAsync(scopeContext.TaskContext).ConfigureAwait(false))
        {
            result = await context.AchievementService.ImportFromUIAFAsync(archive, uiaf.List, strategy).ConfigureAwait(false);
        }

        scopeContext.InfoBarService.Success($"{result}");
        return true;
    }
}