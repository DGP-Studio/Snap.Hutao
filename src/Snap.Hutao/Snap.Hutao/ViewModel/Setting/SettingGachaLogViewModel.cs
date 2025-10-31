// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.UIGF;
using Snap.Hutao.UI.Xaml.View.Dialog;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Setting;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingGachaLogViewModel : Abstraction.ViewModel
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGachaLogRepository gachaLogRepository;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly IUIGFService uigfService;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial SettingGachaLogViewModel(IServiceProvider serviceProvider);

    public partial AppOptions AppOptions { get; }

    [Command("ImportUIGFJsonCommand")]
    private async Task ImportUIGFJsonAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Import UIGF file", "SettingGachaLogViewModel.Command"));

        FileSystemPickerOptions pickerOptions = new()
        {
            Title = SH.ViewModelGachaUIGFImportPickerTitile,
            FilterName = SH.ViewModelGachaLogExportFileType,
            FilterType = "*.json",
        };

        if (fileSystemPickerInteraction.PickFile(pickerOptions) is not (true, { HasValue: true } file))
        {
            return;
        }

        if (await file.DeserializeFromJsonNoThrowAsync<UIGF>(jsonOptions).ConfigureAwait(false) is not (true, { } uigf))
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage));

            return;
        }

        if (uigf.Hk4e.IsDefaultOrEmpty)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelUIGFImportNoHk4eEntry));
            return;
        }

        if (uigf.Hk4e.Select(entry => entry.Uid).ToHashSet().Count != uigf.Hk4e.Length)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelUIGFImportDuplicatedHk4eEntry));
            return;
        }

        UIGFImportDialog importDialog = await contentDialogFactory.CreateInstanceAsync<UIGFImportDialog>(serviceProvider, uigf).ConfigureAwait(false);
        if (await importDialog.GetSelectedUidsAsync().ConfigureAwait(false) is not (true, { } uids))
        {
            return;
        }

        if (uids is null or { Count: 0 })
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelUIGFImportNoSelectedEntry));
            return;
        }

        UIGFImportOptions options = new()
        {
            UIGF = uigf,
            GachaArchiveUids = uids,
        };

        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelUIGFImportingProgressTitle)
            .ConfigureAwait(false);

        using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            try
            {
                await uigfService.ImportAsync(options).ConfigureAwait(false);
                messenger.Send(InfoBarMessage.Success(SH.ViewModelUIGFImportSuccess));
            }
            catch (Exception ex)
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelUIGFImportError, ex));
            }
        }
    }

    [Command("ExportUIGFJsonCommand")]
    private async Task ExportUIGFJsonAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Export UIGF file", "SettingGachaLogViewModel.Command"));

        FileSystemPickerOptions pickerOptions = new()
        {
            Title = SH.ViewModelGachaLogUIGFExportPickerTitle,
            DefaultFileName = "Snap Hutao UIGF.json",
            FilterName = SH.ViewModelGachaLogExportFileType,
            FilterType = "*.json",
        };

        if (fileSystemPickerInteraction.SaveFile(pickerOptions) is not (true, { HasValue: true } file))
        {
            return;
        }

        ImmutableArray<uint> allUids = gachaLogRepository.GetGachaArchiveUidImmutableArray().SelectAsArray(uint.Parse);
        UIGFExportDialog exportDialog = await contentDialogFactory.CreateInstanceAsync<UIGFExportDialog>(serviceProvider, allUids).ConfigureAwait(false);
        if (await exportDialog.GetSelectedUidsAsync().ConfigureAwait(false) is not (true, { } uids))
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
            messenger.Send(InfoBarMessage.Success(SH.ViewModelUIGFExportSuccess));
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelUIGFExportError, ex));
        }
    }
}