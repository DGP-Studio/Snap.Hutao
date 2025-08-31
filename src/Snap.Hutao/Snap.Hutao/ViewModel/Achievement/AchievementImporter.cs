// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Dialog;
using EntityAchievementArchive = Snap.Hutao.Model.Entity.AchievementArchive;

namespace Snap.Hutao.ViewModel.Achievement;

[ConstructorGenerated]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class AchievementImporter
{
    private readonly AchievementImporterScopeContext scopeContext;

    public async ValueTask<bool> FromClipboardAsync(AchievementViewModelScopeContext context)
    {
        if (await context.AchievementService.GetArchiveCollectionAsync().ConfigureAwait(false) is not { CurrentItem: { } archive })
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2);
            return false;
        }

        UIAF? uiaf;
        try
        {
            uiaf = await scopeContext.ClipboardProvider.DeserializeFromJsonAsync<UIAF>().ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(uiaf);
        }
        catch (Exception ex)
        {
            scopeContext.InfoBarService.Error(ex, SH.ViewModelImportFromClipboardErrorTitle);
            return false;
        }

        return await TryImportAsync(context, archive, uiaf).ConfigureAwait(false);
    }

    public async ValueTask<bool> FromEmbeddedYaeAsync(AchievementViewModelScopeContext context)
    {
        if (await context.AchievementService.GetArchiveCollectionAsync().ConfigureAwait(false) is not { CurrentItem: { } archive })
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2);
            return false;
        }

        if (await scopeContext.YaeService.GetAchievementAsync(context.LaunchGameViewModel).ConfigureAwait(false) is not { } uiaf)
        {
            scopeContext.InfoBarService.Warning(SH.ServiceYaeEmbeddedYaeErrorTitle, SH.ViewModelImportByEmbeddedYaeErrorMessage);
            return false;
        }

        return await TryImportAsync(context, archive, uiaf).ConfigureAwait(false);
    }

    public async ValueTask<bool> FromFileAsync(AchievementViewModelScopeContext context)
    {
        if (await context.AchievementService.GetArchiveCollectionAsync().ConfigureAwait(false) is not { CurrentItem: { } archive })
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2);
            return false;
        }

        if (scopeContext.FileSystemPickerInteraction.PickFile(SH.ServiceAchievementUIAFImportPickerTitile, SH.ServiceAchievementUIAFImportPickerFilterText, "*.json") is not (true, { HasValue: true } file))
        {
            return false;
        }

        if (await file.DeserializeFromJsonNoThrowAsync<UIAF>(scopeContext.JsonSerializerOptions).ConfigureAwait(false) is not (true, { } uiaf))
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage);
            return false;
        }

        return await TryImportAsync(context, archive, uiaf).ConfigureAwait(false);
    }

    private async ValueTask<bool> TryImportAsync(AchievementViewModelScopeContext context, EntityAchievementArchive archive, UIAF uiaf)
    {
        if (!uiaf.IsCurrentVersionSupported())
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelAchievementImportWarningMessage);
            return false;
        }

        AchievementImportDialog importDialog = await scopeContext.ContentDialogFactory.CreateInstanceAsync<AchievementImportDialog>(scopeContext.ServiceProvider, uiaf).ConfigureAwait(false);
        if (await importDialog.GetImportStrategyAsync().ConfigureAwait(false) is not (true, var strategy))
        {
            return false;
        }

        ContentDialog dialog = await scopeContext.ContentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelAchievementImportProgress)
            .ConfigureAwait(false);

        using (await scopeContext.ContentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            ImportResult result = await context.AchievementService.ImportFromUIAFAsync(archive, uiaf.List, strategy).ConfigureAwait(false);
            scopeContext.InfoBarService.Success(SH.FormatServiceAchievementImportResult(result.Add, result.Update, result.Remove));
        }

        return true;
    }
}