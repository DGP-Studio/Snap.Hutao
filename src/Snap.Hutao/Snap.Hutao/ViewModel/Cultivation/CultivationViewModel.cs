// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Inventory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Cultivation;

[SuppressMessage("", "CA1001")]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class CultivationViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ICultivationService cultivationService;
    private readonly ILogger<CultivationViewModel> logger;
    private readonly INavigationService navigationService;
    private readonly IInventoryService inventoryService;
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private CancellationTokenSource statisticsCts = new();
    private CultivationMetadataContext? metadataContext;

    public IAdvancedDbCollectionView<CultivateProject>? Projects
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentProjectChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentProjectChanged);
        }
    }

    public ImmutableArray<InventoryItemView> InventoryItems { get; set => SetProperty(ref field, value); } = [];

    public ObservableCollection<CultivateEntryView>? CultivateEntries { get; set => SetProperty(ref field, value); }

    public bool EntriesUpdating { get; set => SetProperty(ref field, value); }

    public bool IncompleteFirst { get; set => SetProperty(ref field, value); }

    public ObservableCollection<StatisticsCultivateItem>? StatisticsItems { get; set => SetProperty(ref field, value); }

    public ResinStatistics? ResinStatistics { get; set => SetProperty(ref field, value); }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        metadataContext = await metadataService.GetContextAsync<CultivationMetadataContext>().ConfigureAwait(false);

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            IAdvancedDbCollectionView<CultivateProject> projects = await cultivationService.GetProjectCollectionAsync().ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Projects = projects;
            Projects.MoveCurrentTo(Projects.SourceCollection.SelectedOrDefault());
        }

        // Force update when re-entering the page
        if (Projects.CurrentItem is not null && CultivateEntries is null)
        {
            await UpdateEntryCollectionAsync(Projects.CurrentItem).ConfigureAwait(false);
        }

        return true;
    }

    protected override void UninitializeOverride()
    {
        using (Projects?.SuppressChangeCurrentItem())
        {
            Projects = default;
        }
    }

    private void OnCurrentProjectChanged(object? sender, object? e)
    {
        UpdateEntryCollectionAsync(Projects?.CurrentItem).SafeForget(logger);
    }

    [Command("AddProjectCommand")]
    private async Task AddProjectAsync()
    {
        CultivateProjectDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivateProjectDialog>().ConfigureAwait(false);
        (bool isOk, CultivateProject project) = await dialog.CreateProjectAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        switch (await cultivationService.TryAddProjectAsync(project).ConfigureAwait(false))
        {
            case ProjectAddResultKind.Added:
                infoBarService.Success(SH.ViewModelCultivationProjectAdded);
                break;
            case ProjectAddResultKind.InvalidName:
                infoBarService.Information(SH.ViewModelCultivationProjectInvalidName);
                break;
            case ProjectAddResultKind.AlreadyExists:
                infoBarService.Information(SH.ViewModelCultivationProjectAlreadyExists);
                break;
            default:
                throw HutaoException.NotSupported();
        }
    }

    [Command("RemoveProjectCommand")]
    private async Task RemoveProjectAsync(CultivateProject? project)
    {
        if (project is null)
        {
            return;
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(
                SH.ViewModelCultivationRemoveProjectTitle,
                SH.ViewModelCultivationRemoveProjectContent)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        await cultivationService.RemoveProjectAsync(project).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        Projects?.MoveCurrentToFirst();
    }

    private async ValueTask UpdateEntryCollectionAsync(CultivateProject? project)
    {
        try
        {
            EntriesUpdating = true;
            if (project is null)
            {
                return;
            }

            CultivationMetadataContext context = await metadataService.GetContextAsync<CultivationMetadataContext>().ConfigureAwait(false);

            ObservableCollection<CultivateEntryView> entries = await cultivationService
                .GetCultivateEntryCollectionAsync(project, context)
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            CultivateEntries = entries;

            await UpdateInventoryItemsAsync().ConfigureAwait(false);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
        finally
        {
            await taskContext.SwitchToMainThreadAsync();
            EntriesUpdating = false;
        }
    }

    [Command("RemoveEntryCommand")]
    private async Task RemoveEntryAsync(CultivateEntryView? entry)
    {
        if (entry is not null)
        {
            ArgumentNullException.ThrowIfNull(CultivateEntries);
            CultivateEntries.Remove(entry);
            await cultivationService.RemoveCultivateEntryAsync(entry.EntryId).ConfigureAwait(false);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("FinishStateCommand")]
    private async Task UpdateFinishedStateAsync(CultivateItemView? item)
    {
        if (item is not null)
        {
            item.IsFinished = !item.IsFinished;
            cultivationService.SaveCultivateItem(item);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("SaveInventoryItemCommand")]
    private async Task SaveInventoryItemAsync(InventoryItemView? inventoryItem)
    {
        if (inventoryItem is not null)
        {
            inventoryService.SaveInventoryItem(inventoryItem);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("RefreshInventoryCommand")]
    private async Task RefreshInventoryAsync()
    {
        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            ContentDialog dialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.ViewModelCultivationRefreshInventoryProgress)
                .ConfigureAwait(false);

            using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
            {
                await inventoryService.RefreshInventoryAsync(metadataContext, Projects.CurrentItem).ConfigureAwait(false);

                await UpdateInventoryItemsAsync().ConfigureAwait(false);
                await UpdateStatisticsItemsAsync().ConfigureAwait(false);
            }
        }
    }

    [Command("ClearInventoryCommand")]
    private async Task ClearInventoryAsync()
    {
        if (Projects?.CurrentItem is null)
        {
            return;
        }

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            ContentDialog dialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.ViewModelCultivationClearInventoryProgress)
                .ConfigureAwait(false);
            using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
            {
                inventoryService.RemoveAllInventoryItem();

                await UpdateInventoryItemsAsync().ConfigureAwait(false);
                await UpdateStatisticsItemsAsync().ConfigureAwait(false);
            }
        }
    }

    [Command("RefreshStatisticsItemsCommand")]
    private async Task UpdateStatisticsItemsAsync()
    {
        // https://github.com/DGP-Studio/Snap.Hutao/issues/2044
        // Force clear the list and bring view to the top to prevent UI dead loop
        await taskContext.SwitchToMainThreadAsync();
        StatisticsItems = null;
        ResinStatistics = null;

        if (Projects?.CurrentItem is null)
        {
            return;
        }

        if (metadataContext is null)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();

        await statisticsCts.CancelAsync().ConfigureAwait(false);
        statisticsCts = new();
        CancellationToken token = statisticsCts.Token;
        ObservableCollection<StatisticsCultivateItem> statistics;
        ResinStatistics resinStatistics;
        try
        {
            statistics = await cultivationService.GetStatisticsCultivateItemCollectionAsync(Projects.CurrentItem, metadataContext, token).ConfigureAwait(false);
            resinStatistics = await cultivationService.GetResinStatisticsAsync(statistics, token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (IncompleteFirst)
        {
            statistics = statistics
                .OrderBy(item => item.IsFinished)
                .ThenByDescending(item => item.IsToday)
                .ThenBy(item => item.Inner.Id, MaterialIdComparer.Shared)
                .ToObservableCollection();
        }

        await taskContext.SwitchToMainThreadAsync();
        StatisticsItems = statistics;
        ResinStatistics = resinStatistics;
    }

    private async ValueTask UpdateInventoryItemsAsync()
    {
        // https://github.com/DGP-Studio/Snap.Hutao/issues/2044
        // Force clear the list and bring view to the top to prevent UI dead loop
        await taskContext.SwitchToMainThreadAsync();
        InventoryItems = [];

        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        InventoryItems = inventoryService.GetInventoryItemViews(metadataContext, Projects.CurrentItem, SaveInventoryItemCommand);
    }

    [Command("NavigateToPageCommand")]
    private void NavigateToPage(string? typeString)
    {
        if (typeString is not null)
        {
            Type? pageType = Type.GetType(typeString);
            ArgumentNullException.ThrowIfNull(pageType);
            navigationService.Navigate(pageType, INavigationCompletionSource.Default, true);
        }
    }
}