// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata;
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
    private static readonly ConcurrentCancellationTokenSource StatisticsCancellationTokenSource = new();

    private readonly ICultivationService cultivationService;
    private readonly ILogger<CultivationViewModel> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    private ObservableCollection<CultivateProject>? projects;
    private CultivateProject? selectedProject;
    private List<InventoryItemView>? inventoryItems;
    private ObservableCollection<CultivateEntryView>? cultivateEntries;
    private ObservableCollection<StatisticsCultivateItem>? statisticsItems;

    /// <summary>
    /// 项目
    /// </summary>
    public ObservableCollection<CultivateProject>? Projects { get => projects; set => SetProperty(ref projects, value); }

    /// <summary>
    /// 当前选中的计划
    /// </summary>
    public CultivateProject? SelectedProject
    {
        get => selectedProject; set
        {
            if (SetProperty(ref selectedProject, value))
            {
                cultivationService.Current = value;
                UpdateEntryCollectionAsync(value).SafeForget(logger);
            }
        }
    }

    /// <summary>
    /// 物品列表
    /// </summary>
    public List<InventoryItemView>? InventoryItems { get => inventoryItems; set => SetProperty(ref inventoryItems, value); }

    /// <summary>
    /// 养成列表
    /// </summary>
    public ObservableCollection<CultivateEntryView>? CultivateEntries { get => cultivateEntries; set => SetProperty(ref cultivateEntries, value); }

    /// <summary>
    /// 统计列表
    /// </summary>
    public ObservableCollection<StatisticsCultivateItem>? StatisticsItems { get => statisticsItems; set => SetProperty(ref statisticsItems, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        bool metaInitialized = await metadataService.InitializeAsync().ConfigureAwait(false);
        if (metaInitialized)
        {
            ObservableCollection<CultivateProject> projects = cultivationService.ProjectCollection;
            CultivateProject? selected = cultivationService.Current;

            await taskContext.SwitchToMainThreadAsync();
            Projects = projects;
            SelectedProject = selected;
        }

        IsInitialized = metaInitialized;
    }

    [Command("AddProjectCommand")]
    private async Task AddProjectAsync()
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();
        CultivateProjectDialog dialog = serviceProvider.CreateInstance<CultivateProjectDialog>();
        (bool isOk, CultivateProject project) = await dialog.CreateProjectAsync().ConfigureAwait(false);

        if (isOk)
        {
            ProjectAddResult result = await cultivationService.TryAddProjectAsync(project).ConfigureAwait(false);
            IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

            switch (result)
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
    }

    [Command("RemoveProjectCommand")]
    private async Task RemoveProjectAsync(CultivateProject? project)
    {
        if (project != null)
        {
            await cultivationService.RemoveProjectAsync(project).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            SelectedProject = Projects!.FirstOrDefault();
        }
    }

    private async Task UpdateEntryCollectionAsync(CultivateProject? project)
    {
        if (project != null)
        {
            List<Material> materials = await metadataService.GetMaterialsAsync().ConfigureAwait(false);

            ObservableCollection<CultivateEntryView> entries = await cultivationService
                .GetCultivateEntriesAsync(project)
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            CultivateEntries = entries;
            InventoryItems = cultivationService.GetInventoryItems(project, materials, SaveInventoryItemCommand);

            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("RemoveEntryCommand")]
    private async Task RemoveEntryAsync(CultivateEntryView? entry)
    {
        if (entry != null)
        {
            CultivateEntries!.Remove(entry);
            await cultivationService.RemoveCultivateEntryAsync(entry.EntryId).ConfigureAwait(false);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("SaveInventoryItemCommand")]
    private void SaveInventoryItem(InventoryItemView? inventoryItem)
    {
        if (inventoryItem != null)
        {
            cultivationService.SaveInventoryItem(inventoryItem);
            UpdateStatisticsItemsAsync().SafeForget();
        }
    }

    [Command("FinishStateCommand")]
    private void UpdateFinishedState(CultivateItemView? item)
    {
        if (item != null)
        {
            item.IsFinished = !item.IsFinished;
            cultivationService.SaveCultivateItem(item.Entity);
            UpdateStatisticsItemsAsync().SafeForget();
        }
    }

    private async Task UpdateStatisticsItemsAsync()
    {
        if (SelectedProject != null)
        {
            await taskContext.SwitchToBackgroundAsync();
            CancellationToken token = StatisticsCancellationTokenSource.CancelPreviousOne();
            ObservableCollection<StatisticsCultivateItem> statistics;
            try
            {
                statistics = await cultivationService.GetStatisticsCultivateItemCollectionAsync(SelectedProject, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            StatisticsItems = statistics;
        }
    }

    [Command("NavigateToPageCommand")]
    private void NavigateToPage(string? typeString)
    {
        if (typeString != null)
        {
            serviceProvider
                .GetRequiredService<INavigationService>()
                .Navigate(Type.GetType(typeString)!, INavigationAwaiter.Default, true);
        }
    }
}