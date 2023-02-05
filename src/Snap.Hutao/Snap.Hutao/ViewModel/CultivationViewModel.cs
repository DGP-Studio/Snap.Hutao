// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Model.Binding.Cultivation;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 养成视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class CultivationViewModel : Abstraction.ViewModel
{
    private readonly ICultivationService cultivationService;
    private readonly IInfoBarService infoBarService;
    private readonly IMetadataService metadataService;
    private readonly ILogger<CultivationViewModel> logger;

    private readonly ConcurrentCancellationTokenSource statisticsCancellationTokenSource = new();

    private ObservableCollection<CultivateProject>? projects;
    private CultivateProject? selectedProject;
    private List<Model.Binding.Inventory.InventoryItem>? inventoryItems;
    private ObservableCollection<Model.Binding.Cultivation.CultivateEntry>? cultivateEntries;
    private ObservableCollection<StatisticsCultivateItem>? statisticsItems;

    /// <summary>
    /// 构造一个新的养成视图模型
    /// </summary>
    /// <param name="cultivationService">养成服务</param>
    /// <param name="infoBarService">信息服务</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="logger">日志器</param>
    public CultivationViewModel(
        ICultivationService cultivationService,
        IInfoBarService infoBarService,
        IMetadataService metadataService,
        ILogger<CultivationViewModel> logger)
    {
        this.cultivationService = cultivationService;
        this.infoBarService = infoBarService;
        this.metadataService = metadataService;
        this.logger = logger;

        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
        AddProjectCommand = new AsyncRelayCommand(AddProjectAsync);
        RemoveProjectCommand = new AsyncRelayCommand<CultivateProject>(RemoveProjectAsync);
        RemoveEntryCommand = new AsyncRelayCommand<Model.Binding.Cultivation.CultivateEntry>(RemoveEntryAsync);
        SaveInventoryItemCommand = new RelayCommand<Model.Binding.Inventory.InventoryItem>(SaveInventoryItem);
        NavigateToPageCommand = new RelayCommand<string>(NavigateToPage);
        FinishStateCommand = new RelayCommand<Model.Binding.Cultivation.CultivateItem>(FlipFinishedState);
    }

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
                if (value != null)
                {
                    UpdateEntryCollectionAsync(value).SafeForget(logger);
                }
            }
        }
    }

    /// <summary>
    /// 物品列表
    /// </summary>
    public List<Model.Binding.Inventory.InventoryItem>? InventoryItems { get => inventoryItems; set => SetProperty(ref inventoryItems, value); }

    /// <summary>
    /// 养成列表
    /// </summary>
    public ObservableCollection<Model.Binding.Cultivation.CultivateEntry>? CultivateEntries { get => cultivateEntries; set => SetProperty(ref cultivateEntries, value); }

    /// <summary>
    /// 统计列表
    /// </summary>
    public ObservableCollection<StatisticsCultivateItem>? StatisticsItems { get => statisticsItems; set => SetProperty(ref statisticsItems, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 添加项目命令
    /// </summary>
    public ICommand AddProjectCommand { get; }

    /// <summary>
    /// 删除项目命令
    /// </summary>
    public ICommand RemoveProjectCommand { get; }

    /// <summary>
    /// 移除
    /// </summary>
    public ICommand RemoveEntryCommand { get; }

    /// <summary>
    /// 保存物品命令
    /// </summary>
    public ICommand SaveInventoryItemCommand { get; }

    /// <summary>
    /// 导航到指定的页面命令
    /// </summary>
    public ICommand NavigateToPageCommand { get; set; }

    /// <summary>
    /// 调整完成状态命令
    /// </summary>
    public ICommand FinishStateCommand { get; }

    private async Task OpenUIAsync()
    {
        bool metaInitialized = await metadataService.InitializeAsync().ConfigureAwait(true);
        if (metaInitialized)
        {
            Projects = cultivationService.GetProjectCollection();
            SelectedProject = cultivationService.Current;
        }

        IsInitialized = metaInitialized;
    }

    private async Task AddProjectAsync()
    {
        // ContentDialog must be created by main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        (bool isOk, CultivateProject project) = await new CultivateProjectDialog().CreateProjectAsync().ConfigureAwait(false);

        if (isOk)
        {
            ProjectAddResult result = await cultivationService.TryAddProjectAsync(project).ConfigureAwait(false);

            switch (result)
            {
                case ProjectAddResult.Added:
                    infoBarService.Success(SH.ViewModelCultivationProjectAdded);
                    await ThreadHelper.SwitchToMainThreadAsync();
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

    private async Task RemoveProjectAsync(CultivateProject? project)
    {
        if (project != null)
        {
            await cultivationService.RemoveProjectAsync(project).ConfigureAwait(false);

            await ThreadHelper.SwitchToMainThreadAsync();
            SelectedProject = Projects!.FirstOrDefault();
        }
    }

    private async Task UpdateEntryCollectionAsync(CultivateProject? project)
    {
        if (project != null)
        {
            List<Model.Metadata.Material> materials = await metadataService.GetMaterialsAsync().ConfigureAwait(false);
            Dictionary<Model.Primitive.AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            Dictionary<Model.Primitive.WeaponId, Model.Metadata.Weapon.Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);

            ObservableCollection<Model.Binding.Cultivation.CultivateEntry> entries = await cultivationService
                .GetCultivateEntriesAsync(project)
                .ConfigureAwait(false);

            await ThreadHelper.SwitchToMainThreadAsync();
            CultivateEntries = entries;
            InventoryItems = cultivationService.GetInventoryItems(project, materials);

            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    private async Task RemoveEntryAsync(Model.Binding.Cultivation.CultivateEntry? entry)
    {
        if (entry != null)
        {
            CultivateEntries!.Remove(entry);
            await cultivationService.RemoveCultivateEntryAsync(entry.EntryId).ConfigureAwait(false);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    private void SaveInventoryItem(Model.Binding.Inventory.InventoryItem? inventoryItem)
    {
        if (inventoryItem != null)
        {
            cultivationService.SaveInventoryItem(inventoryItem);
            UpdateStatisticsItemsAsync().SafeForget();
        }
    }

    private void FlipFinishedState(Model.Binding.Cultivation.CultivateItem? item)
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
            await ThreadHelper.SwitchToBackgroundAsync();
            CancellationToken token = statisticsCancellationTokenSource.Register();
            ObservableCollection<StatisticsCultivateItem> statistics;
            try
            {
                statistics = await cultivationService.GetStatisticsCultivateItemCollectionAsync(SelectedProject, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await ThreadHelper.SwitchToMainThreadAsync();
            StatisticsItems = statistics;
        }
    }

    private void NavigateToPage(string? typeString)
    {
        if (typeString != null)
        {
            Type? pageType = Type.GetType(typeString);
            Ioc.Default.GetRequiredService<INavigationService>().Navigate(pageType!, INavigationAwaiter.Default, true);
        }
    }
}