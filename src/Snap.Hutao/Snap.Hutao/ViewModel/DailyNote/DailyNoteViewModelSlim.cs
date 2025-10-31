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

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class DailyNoteViewModelSlim : Abstraction.ViewModelSlim<DailyNotePage>
{
    private readonly IDailyNoteService dailyNoteService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private DailyNoteMetadataContext? metadataContext;

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial DailyNoteViewModelSlim(IServiceProvider serviceProvider);

    // This property must be a reference type
    [ObservableProperty]
    public partial List<DailyNoteEntry>? DailyNoteEntries { get; set; }

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

            // We must make a copy of the entries collection to avoid the following exception:
            // Element is already the child of another element.
            DailyNoteEntries = [.. entries];
            IsInitialized = true;
        }
        catch (HutaoException ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }
}