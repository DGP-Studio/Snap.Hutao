// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.UIGF;
using Snap.Hutao.UI.Xaml.View.Dialog;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingGachaLogViewModel : Abstraction.ViewModel
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGachaLogRepository gachaLogRepository;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IInfoBarService infoBarService;
    private readonly IUIGFService uigfService;
    private readonly AppOptions appOptions;

    public AppOptions AppOptions { get => appOptions; }

    [Command("ImportUIGFJsonCommand")]
    private async Task ImportUIGFJsonAsync()
    {
        (bool isOk, ValueFile file) = fileSystemPickerInteraction.PickFile(
            SH.ViewModelGachaUIGFImportPickerTitile,
            [(SH.ViewModelGachaLogExportFileType, "*.json")]);

        if (!isOk)
        {
            return;
        }

        ValueResult<bool, UIGF?> result = await file.DeserializeFromJsonAsync<UIGF>(jsonOptions).ConfigureAwait(false);
        if (!result.TryGetValue(out UIGF? uigf))
        {
            infoBarService.Error(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage);
            return;
        }

        if (uigf.Hk4e is null or [])
        {
            infoBarService.Warning(SH.ViewModelUIGFImportNoHk4eEntry);
            return;
        }

        if (uigf.Hk4e.Select(entry => entry.Uid).ToHashSet().Count != uigf.Hk4e.Count)
        {
            infoBarService.Warning(SH.ViewModelUIGFImportDuplicatedHk4eEntry);
            return;
        }

        UIGFImportDialog importDialog = await contentDialogFactory.CreateInstanceAsync<UIGFImportDialog>(uigf).ConfigureAwait(false);
        (bool isOk2, HashSet<uint> uids) = await importDialog.GetSelectedUidsAsync().ConfigureAwait(false);
        if (!isOk2)
        {
            return;
        }

        if (uids is null or { Count: 0 })
        {
            infoBarService.Warning(SH.ViewModelUIGFImportNoSelectedEntry);
            return;
        }

        UIGFImportOptions options = new()
        {
            UIGF = uigf,
            GachaArchiveUids = uids,
        };

        try
        {
            await uigfService.ImportAsync(options).ConfigureAwait(false);
            infoBarService.Success(SH.ViewModelUIGFImportSuccess);
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex, SH.ViewModelUIGFImportError);
        }
    }

    [Command("ExportUIGFJsonCommand")]
    private async Task ExportUIGFJsonAsync()
    {
        (bool isOk, ValueFile file) = fileSystemPickerInteraction.SaveFile(
            SH.ViewModelGachaLogUIGFExportPickerTitle,
            $"Snap Hutao UIGF.json",
            [(SH.ViewModelGachaLogExportFileType, "*.json")]);

        if (!isOk)
        {
            return;
        }

        List<uint> allUids = gachaLogRepository.GetGachaArchiveUidList().SelectList(uint.Parse);
        UIGFExportDialog exportDialog = await contentDialogFactory.CreateInstanceAsync<UIGFExportDialog>(allUids).ConfigureAwait(false);

        (bool isOk2, List<uint> uids) = await exportDialog.GetSelectedUidsAsync().ConfigureAwait(false);
        if (!isOk2)
        {
            return;
        }

        UIGFExportOptions options = new()
        {
            FilePath = file,
            GachaArchiveUids = uids,
        };

        try
        {
            await uigfService.ExportAsync(options).ConfigureAwait(false);
            infoBarService.Success(SH.ViewModelUIGFExportSuccess);
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex, SH.ViewModelUIGFExportError);
        }
    }
}