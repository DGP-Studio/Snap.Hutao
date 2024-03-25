// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
            case ProjectAddResult.Added:
                infoBarService.Success(SH.ViewModelCultivationProjectAdded);
                await taskContext.SwitchToMainThreadAsync();
                SelectedProject = project;
                break;
            case ProjectAddResult.InvalidName:
                infoBarService.Information(SH.ViewModelCultivationProjectInvalidName);
                break;
            case ProjectAddResult.AlreadyExists:
                infoBarService.Information(SH.ViewModelCultivationProjectAlreadyExists);
                break;
            default:
                throw Must.NeverHappen();
        }
    }

    [Command("RemoveProjectCommand")]
    private async Task RemoveProjectAsync(CultivateProject? project)
    {
        if (project is null)
        {
            return;
        }

        await cultivationService.RemoveProjectAsync(project).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        ArgumentNullException.ThrowIfNull(Projects);
        SelectedProject = Projects.FirstOrDefault();
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
        InventoryItems = cultivationService.GetInventoryItemViews(project, context, SaveInventoryItemCommand);

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
            cultivationService.SaveInventoryItem(inventoryItem);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
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
                if (statistics is not null)
                {
                    statistics = this.SortStatistics(statistics);
                    foreach (var item in statistics)
                    {
                        Debug.Print("Name: {0}, Id: {1}, Rank: {2}, Material: {3}, Item: {4}",
                            item.Inner.Name, item.Inner.Id.Value, item.Inner.RankLevel, item.Inner.MaterialType, item.Inner.ItemType);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            StatisticsItems = statistics;
        }
    }

    private ObservableCollection<StatisticsCultivateItem> SortStatistics(ObservableCollection<StatisticsCultivateItem> statistics)
    {
        return statistics.Order(new StatisticsCaltivateItemComparer()).ToObservableCollection();
    }

    private class StatisticsCaltivateItemComparer: IComparer<StatisticsCultivateItem>
    {
        public int Compare(StatisticsCultivateItem? x, StatisticsCultivateItem? y)
        {
            // TODO: 理论上的最优解：先通过观测枢获取所有背包物品，然后根据filter字段依次分类，先按这个类别做排序，然后再按品质等进行排序
            // 不仅如此，以后想按照材料类型分类的话，这也是必做的。

            // 对null做判定，防止IDE警告
            if (x is null) { return -1; }
            if (y is null) { return -1; }

            // 摩拉、矿、经验书全局只出现一次，放在最前面
            if (x.Inner.Name == "摩拉") { return -1; }
            if (y.Inner.Name == "摩拉") { return 1; }
            if (x.Inner.Name == "精锻用魔矿") { return -1; }
            if (y.Inner.Name == "精锻用魔矿") { return 1; }
            if (x.Inner.Name == "大英雄的经验") { return -1; }
            if (y.Inner.Name == "大英雄的经验") { return 1; }

            // 剩下的物品暂时按照id排序，更细致的排序策略以后再说
            return (int)x.Inner.Id.Value - (int)y.Inner.Id.Value;
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