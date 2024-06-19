// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Inventory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Cultivation;

/// <summary>
/// 养成视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class CultivationViewModel : Abstraction.ViewModel
{
    private readonly ConcurrentCancellationTokenSource statisticsCancellationTokenSource = new();

    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ICultivationService cultivationService;
    private readonly ILogger<CultivationViewModel> logger;
    private readonly INavigationService navigationService;
    private readonly IInventoryService inventoryService;
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private ObservableCollection<CultivateProject>? projects;
    private CultivateProject? selectedProject;
    private List<InventoryItemView>? inventoryItems;
    private ObservableCollection<CultivateEntryView>? cultivateEntries;
    private ObservableCollection<StatisticsCultivateItem>? statisticsItems;

    public ObservableCollection<CultivateProject>? Projects { get => projects; set => SetProperty(ref projects, value); }

    public CultivateProject? SelectedProject
    {
        get => selectedProject; set
        {
            if (SetProperty(ref selectedProject, value) && !IsViewDisposed)
            {
                cultivationService.Current = value;
                UpdateEntryCollectionAsync(value).SafeForget(logger);
            }
        }
    }

    public List<InventoryItemView>? InventoryItems { get => inventoryItems; set => SetProperty(ref inventoryItems, value); }

    public ObservableCollection<CultivateEntryView>? CultivateEntries { get => cultivateEntries; set => SetProperty(ref cultivateEntries, value); }

    public ObservableCollection<StatisticsCultivateItem>? StatisticsItems { get => statisticsItems; set => SetProperty(ref statisticsItems, value); }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            ObservableCollection<CultivateProject> projects = cultivationService.ProjectCollection;
            CultivateProject? selected = cultivationService.Current;

            await taskContext.SwitchToMainThreadAsync();
            Projects = projects;
            SelectedProject = selected;
            return true;
        }

        return false;
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
                await taskContext.SwitchToMainThreadAsync();
                SelectedProject = project;
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
            .CreateForConfirmCancelAsync(SH.ViewModelCultivationRemoveProjectTitle, SH.ViewModelCultivationRemoveProjectContent)
            .ConfigureAwait(false);

        if (result is ContentDialogResult.Primary)
        {
            await cultivationService.RemoveProjectAsync(project).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            ArgumentNullException.ThrowIfNull(Projects);
            SelectedProject = Projects.FirstOrDefault();
        }
    }

    private async ValueTask UpdateEntryCollectionAsync(CultivateProject? project)
    {
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
        if (SelectedProject is null)
        {
            return;
        }

        using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
        {
            ContentDialog dialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.ViewModelCultivationRefreshInventoryProgress)
                .ConfigureAwait(false);

            using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
            {
                await inventoryService.RefreshInventoryAsync(SelectedProject).ConfigureAwait(false);

                await UpdateInventoryItemsAsync().ConfigureAwait(false);
                await UpdateStatisticsItemsAsync().ConfigureAwait(false);
            }
        }
    }

    private async ValueTask UpdateStatisticsItemsAsync()
    {
        if (SelectedProject is not null)
        {
            await taskContext.SwitchToBackgroundAsync();

            CancellationToken token = statisticsCancellationTokenSource.Register();
            ObservableCollection<StatisticsCultivateItem> statistics;
            try
            {
                CultivationMetadataContext context = await metadataService.GetContextAsync<CultivationMetadataContext>().ConfigureAwait(false);
                statistics = await cultivationService.GetStatisticsCultivateItemCollectionAsync(SelectedProject, context, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            StatisticsItems = statistics;
        }
    }

    private async ValueTask UpdateInventoryItemsAsync()
    {
        if (SelectedProject is not null)
        {
            await taskContext.SwitchToBackgroundAsync();
            CultivationMetadataContext context = await metadataService.GetContextAsync<CultivationMetadataContext>().ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            InventoryItems = inventoryService.GetInventoryItemViews(SelectedProject, context, SaveInventoryItemCommand);
        }
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