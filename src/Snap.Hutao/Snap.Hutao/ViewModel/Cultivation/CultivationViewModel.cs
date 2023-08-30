// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
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
            if (SetProperty(ref selectedProject, value) && !IsViewDisposed)
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

        if (isOk)
        {
            ProjectAddResult result = await cultivationService.TryAddProjectAsync(project).ConfigureAwait(false);

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
        if (project is not null)
        {
            await cultivationService.RemoveProjectAsync(project).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            ArgumentNullException.ThrowIfNull(Projects);
            SelectedProject = Projects.FirstOrDefault();
        }
    }

    private async ValueTask UpdateEntryCollectionAsync(CultivateProject? project)
    {
        if (project is not null)
        {
            List<Material> materials = await metadataService.GetMaterialsAsync().ConfigureAwait(false);
            Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);

            ObservableCollection<CultivateEntryView> entries = await cultivationService
                .GetCultivateEntriesAsync(project, materials, idAvatarMap, idWeaponMap)
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            CultivateEntries = entries;
            InventoryItems = cultivationService.GetInventoryItemViews(project, materials, SaveInventoryItemCommand);

            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
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
                List<Material> materials = await metadataService.GetMaterialsAsync(token).ConfigureAwait(false);
                statistics = await cultivationService.GetStatisticsCultivateItemCollectionAsync(SelectedProject, materials, token).ConfigureAwait(false);
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
        if (typeString is not null)
        {
            Type? pageType = Type.GetType(typeString);
            ArgumentNullException.ThrowIfNull(pageType);
            navigationService.Navigate(pageType, INavigationAwaiter.Default, true);
        }
    }
}