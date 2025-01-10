// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Page;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.DailyNote;

[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class DailyNoteViewModelSlim : Abstraction.ViewModelSlim<DailyNotePage>
{
    private readonly IDailyNoteService dailyNoteService;
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private DailyNoteMetadataContext? metadataContext;

    [ObservableProperty]
    public partial ObservableCollection<DailyNoteEntry>? DailyNoteEntries { get; set; }

    /// <inheritdoc/>
    protected override async Task LoadAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return;
        }

        metadataContext = await metadataService.GetContextAsync<DailyNoteMetadataContext>().ConfigureAwait(false);

        try
        {
            await taskContext.SwitchToBackgroundAsync();
            ObservableCollection<DailyNoteEntry> entries = await dailyNoteService
                .GetDailyNoteEntryCollectionAsync(metadataContext)
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            DailyNoteEntries = entries;
            IsInitialized = true;
        }
        catch (HutaoException ex)
        {
            infoBarService.Error(ex);
        }
    }
}