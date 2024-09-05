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
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.View.Dialog;
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

    private CancellationTokenSource statisticsCancellationTokenSource = new();

    private AdvancedDbCollectionView<CultivateProject>? projects;
    private List<InventoryItemView>? inventoryItems;
    private ObservableCollection<CultivateEntryView>? cultivateEntries;
    private ObservableCollection<StatisticsCultivateItem>? statisticsItems;
    private bool entriesUpdating;

    public AdvancedDbCollectionView<CultivateProject>? Projects
    {
        get => projects;
        set
        {
            if (projects is not null)
            {
                projects.CurrentChanged -= OnCurrentProjectChanged;
            }

            SetProperty(ref projects, value);

            if (value is not null)
            {
                value.CurrentChanged += OnCurrentProjectChanged;
            }
        }
    }

    public List<InventoryItemView>? InventoryItems { get => inventoryItems; set => SetProperty(ref inventoryItems, value); }

    public ObservableCollection<CultivateEntryView>? CultivateEntries { get => cultivateEntries; set => SetProperty(ref cultivateEntries, value); }

    public bool EntriesUpdating { get => entriesUpdating; set => SetProperty(ref entriesUpdating, value); }

    public ObservableCollection<StatisticsCultivateItem>? StatisticsItems { get => statisticsItems; set => SetProperty(ref statisticsItems, value); }

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                await taskContext.SwitchToMainThreadAsync();
                Projects = cultivationService.Projects;
                Projects.MoveCurrentTo(Projects.SourceCollection.SelectedOrDefault());
            }

            // Force update when re-entering the page
            if (Projects.CurrentItem is not null && CultivateEntries is null)
            {
                await UpdateEntryCollectionAsync(Projects.CurrentItem).ConfigureAwait(false);
            }

            return true;
        }

        return false;
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
                .GetCultivateEntriesAsync(project, context)
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
        if (Projects?.CurrentItem is null)
        {
            return;
        }

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            ContentDialog dialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.ViewModelCultivationRefreshInventoryProgress)
                .ConfigureAwait(false);

            using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
            {
                await inventoryService.RefreshInventoryAsync(Projects.CurrentItem).ConfigureAwait(false);

                await UpdateInventoryItemsAsync().ConfigureAwait(false);
                await UpdateStatisticsItemsAsync().ConfigureAwait(false);
            }
        }
    }

    private async ValueTask UpdateStatisticsItemsAsync()
    {
        if (Projects?.CurrentItem is null)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();

        statisticsCancellationTokenSource.Cancel();
        statisticsCancellationTokenSource = new();
        CancellationToken token = statisticsCancellationTokenSource.Token;
        ObservableCollection<StatisticsCultivateItem> statistics;
        try
        {
            CultivationMetadataContext context = await metadataService.GetContextAsync<CultivationMetadataContext>().ConfigureAwait(false);
            statistics = await cultivationService.GetStatisticsCultivateItemCollectionAsync(Projects.CurrentItem, context, token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        StatisticsItems = statistics;
    }

    private async ValueTask UpdateInventoryItemsAsync()
    {
        if (Projects?.CurrentItem is null)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        CultivationMetadataContext context = await metadataService.GetContextAsync<CultivationMetadataContext>().ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        InventoryItems = inventoryService.GetInventoryItemViews(Projects.CurrentItem, context, SaveInventoryItemCommand);
    }

    [Command("NavigateToPageCommand")]
    private void NavigateToPage(string? typeString)
    {
        if (typeString is not null)
        {
            Type? pageType = Type.GetType(typeString);
            ArgumentNullException.ThrowIfNull(pageType);
            navigationService.Navigate(pageType, INavigationAwaiter.Default, true);
        }
    }
}