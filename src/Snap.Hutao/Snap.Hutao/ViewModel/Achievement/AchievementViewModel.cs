// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;
using BindingAchievement = Snap.Hutao.Model.Binding.Achievement.Achievement;
using BindingAchievementGoal = Snap.Hutao.Model.Binding.Achievement.AchievementGoal;
using EntityAchievementArchive = Snap.Hutao.Model.Entity.AchievementArchive;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;
using MetadataAchievementGoal = Snap.Hutao.Model.Metadata.Achievement.AchievementGoal;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 成就视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class AchievementViewModel : Abstraction.ViewModel, INavigationRecipient
{
    private static readonly SortDescription IncompletedItemsFirstSortDescription = new(nameof(BindingAchievement.IsChecked), SortDirection.Ascending);
    private static readonly SortDescription CompletionTimeSortDescription = new(nameof(BindingAchievement.Time), SortDirection.Descending);

    private readonly IAchievementService achievementService;
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly JsonSerializerOptions options;

    private readonly AchievementFinishPercentUpdater achievementFinishPercentUpdater;
    private readonly AchievementImporter achievementImporter;

    private readonly TaskCompletionSource<bool> openUICompletionSource = new();

    private AdvancedCollectionView? achievements;
    private List<BindingAchievementGoal>? achievementGoals;
    private BindingAchievementGoal? selectedAchievementGoal;
    private ObservableCollection<EntityAchievementArchive>? archives;
    private EntityAchievementArchive? selectedArchive;
    private bool isIncompletedItemsFirst = true;
    private string searchText = string.Empty;
    private string? finishDescription;

    /// <summary>
    /// 构造一个新的成就视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="achievementService">成就服务</param>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="contentDialogFactory">内容对话框工厂</param>
    /// <param name="messenger">消息器</param>
    public AchievementViewModel(
        IServiceProvider serviceProvider,
        IMetadataService metadataService,
        IAchievementService achievementService,
        IInfoBarService infoBarService,
        JsonSerializerOptions options,
        IContentDialogFactory contentDialogFactory,
        IMessenger messenger)
    {
        this.metadataService = metadataService;
        this.achievementService = achievementService;
        this.infoBarService = infoBarService;
        this.contentDialogFactory = contentDialogFactory;
        this.options = options;

        achievementFinishPercentUpdater = new(this);
        achievementImporter = new(serviceProvider);

        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
        ImportUIAFFromClipboardCommand = new AsyncRelayCommand(ImportUIAFFromClipboardAsync);
        ImportUIAFFromFileCommand = new AsyncRelayCommand(ImportUIAFFromFileAsync);
        ExportAsUIAFToFileCommand = new AsyncRelayCommand(ExportAsUIAFToFileAsync);
        AddArchiveCommand = new AsyncRelayCommand(AddArchiveAsync);
        RemoveArchiveCommand = new AsyncRelayCommand(RemoveArchiveAsync);
        SearchAchievementCommand = new RelayCommand<string>(UpdateAchievementsFilterBySerach);
        SortIncompletedSwitchCommand = new RelayCommand(UpdateAchievementsSort);
        SaveAchievementCommand = new RelayCommand<BindingAchievement>(SaveAchievement);
    }

    /// <summary>
    /// 成就存档集合
    /// </summary>
    public ObservableCollection<EntityAchievementArchive>? Archives
    {
        get => archives;
        set => SetProperty(ref archives, value);
    }

    /// <summary>
    /// 选中的成就存档
    /// </summary>
    public EntityAchievementArchive? SelectedArchive
    {
        get => selectedArchive;
        set
        {
            if (SetProperty(ref selectedArchive, value))
            {
                achievementService.CurrentArchive = value;
                if (value != null)
                {
                    UpdateAchievementsAsync(value).SafeForget();
                }
            }
        }
    }

    /// <summary>
    /// 成就视图
    /// </summary>
    public AdvancedCollectionView? Achievements
    {
        get => achievements;
        set => SetProperty(ref achievements, value);
    }

    /// <summary>
    /// 成就分类
    /// </summary>
    public List<BindingAchievementGoal>? AchievementGoals
    {
        get => achievementGoals;
        set => SetProperty(ref achievementGoals, value);
    }

    /// <summary>
    /// 选中的成就分类
    /// </summary>
    public BindingAchievementGoal? SelectedAchievementGoal
    {
        get => selectedAchievementGoal;
        set
        {
            SetProperty(ref selectedAchievementGoal, value);
            SearchText = string.Empty;
            UpdateAchievementsFilterByGoal(value);
        }
    }

    /// <summary>
    /// 搜索文本
    /// </summary>
    public string SearchText
    {
        get => searchText;
        set => SetProperty(ref searchText, value);
    }

    /// <summary>
    /// 未完成优先
    /// </summary>
    public bool IsIncompletedItemsFirst
    {
        get => isIncompletedItemsFirst;
        set => SetProperty(ref isIncompletedItemsFirst, value);
    }

    /// <summary>
    /// 完成进度描述
    /// </summary>
    public string? FinishDescription
    {
        get => finishDescription;
        set => SetProperty(ref finishDescription, value);
    }

    /// <summary>
    /// 打开页面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 添加存档命令
    /// </summary>
    public ICommand AddArchiveCommand { get; }

    /// <summary>
    /// 删除存档命令
    /// </summary>
    public ICommand RemoveArchiveCommand { get; }

    /// <summary>
    /// 搜索成就命令
    /// </summary>
    public ICommand SearchAchievementCommand { get; }

    /// <summary>
    /// 从剪贴板导入UIAF命令
    /// </summary>
    public ICommand ImportUIAFFromClipboardCommand { get; }

    /// <summary>
    /// 从文件导入UIAF命令
    /// </summary>
    public ICommand ImportUIAFFromFileCommand { get; }

    /// <summary>
    /// 以 UIAF 文件格式导出
    /// </summary>
    public ICommand ExportAsUIAFToFileCommand { get; }

    /// <summary>
    /// 筛选未完成项开关命令
    /// </summary>
    public ICommand SortIncompletedSwitchCommand { get; }

    /// <summary>
    /// 保存单个成就命令
    /// </summary>
    public ICommand SaveAchievementCommand { get; }

    /// <inheritdoc/>
    public async Task<bool> ReceiveAsync(INavigationData data)
    {
        if (await openUICompletionSource.Task.ConfigureAwait(false))
        {
            if (data.Data is Activation.ImportUIAFFromClipBoard)
            {
                await ImportUIAFFromClipboardAsync().ConfigureAwait(false);
                return true;
            }
        }

        return false;
    }

    private async Task OpenUIAsync()
    {
        bool metaInitialized = await metadataService.InitializeAsync().ConfigureAwait(false);

        if (metaInitialized)
        {
            try
            {
                List<BindingAchievementGoal> sortedGoals;
                ObservableCollection<EntityAchievementArchive> archives;

                using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                {
                    List<MetadataAchievementGoal> goals = await metadataService.GetAchievementGoalsAsync(CancellationToken).ConfigureAwait(false);
                    sortedGoals = goals
                        .OrderBy(goal => goal.Order)
                        .Select(goal => new BindingAchievementGoal(goal))
                        .ToList();
                    archives = await achievementService.GetArchiveCollectionAsync().ConfigureAwait(false);
                }

                await ThreadHelper.SwitchToMainThreadAsync();
                AchievementGoals = sortedGoals;
                Archives = archives;
                SelectedArchive = Archives.SelectedOrDefault();

                IsInitialized = true;
            }
            catch (OperationCanceledException)
            {
                // User canceled the loading operation,
                // Indicate initialization not succeed.
                openUICompletionSource.TrySetResult(false);
                return;
            }
        }

        openUICompletionSource.TrySetResult(metaInitialized);
    }

    private async Task AddArchiveAsync()
    {
        if (Archives != null)
        {
            // ContentDialog must be created by main thread.
            await ThreadHelper.SwitchToMainThreadAsync();
            (bool isOk, string name) = await new AchievementArchiveCreateDialog().GetInputAsync().ConfigureAwait(false);

            if (isOk)
            {
                ArchiveAddResult result = await achievementService.TryAddArchiveAsync(EntityAchievementArchive.Create(name)).ConfigureAwait(false);

                switch (result)
                {
                    case ArchiveAddResult.Added:
                        await ThreadHelper.SwitchToMainThreadAsync();
                        SelectedArchive = Archives.SingleOrDefault(a => a.Name == name);
                        infoBarService.Success(string.Format(SH.ViewModelAchievementArchiveAdded, name));
                        break;
                    case ArchiveAddResult.InvalidName:
                        infoBarService.Information(SH.ViewModelAchievementArchiveInvalidName);
                        break;
                    case ArchiveAddResult.AlreadyExists:
                        infoBarService.Information(string.Format(SH.ViewModelAchievementArchiveAlreadyExists, name));
                        break;
                    default:
                        throw Must.NeverHappen();
                }
            }
        }
    }

    private async Task RemoveArchiveAsync()
    {
        if (Archives != null && SelectedArchive != null)
        {
            ContentDialogResult result = await contentDialogFactory
                .ConfirmCancelAsync(
                    string.Format(SH.ViewModelAchievementRemoveArchiveTitle, SelectedArchive.Name),
                    SH.ViewModelAchievementRemoveArchiveContent)
                .ConfigureAwait(false);

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                    {
                        await achievementService.RemoveArchiveAsync(SelectedArchive).ConfigureAwait(false);
                    }

                    // Re-select first archive
                    await ThreadHelper.SwitchToMainThreadAsync();
                    SelectedArchive = Archives.FirstOrDefault();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }

    private async Task ExportAsUIAFToFileAsync()
    {
        if (SelectedArchive != null && Achievements != null)
        {
            FileSavePicker picker = Ioc.Default.GetRequiredService<IPickerFactory>().GetFileSavePicker();
            picker.FileTypeChoices.Add(SH.ViewModelAchievementExportFileType, ".json".Enumerate().ToList());
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.CommitButtonText = SH.FilePickerExportCommit;
            picker.SuggestedFileName = $"{achievementService.CurrentArchive?.Name}.json";

            (bool isPickerOk, FilePath file) = await picker.TryPickSaveFileAsync().ConfigureAwait(false);
            if (isPickerOk)
            {
                UIAF uiaf = await achievementService.ExportToUIAFAsync(SelectedArchive).ConfigureAwait(false);
                bool isOk = await file.SerializeToJsonAsync(uiaf, options).ConfigureAwait(false);

                if (isOk)
                {
                    infoBarService.Success(SH.ViewModelExportSuccessTitle, SH.ViewModelExportSuccessMessage);
                }
                else
                {
                    infoBarService.Warning(SH.ViewModelExportWarningTitle, SH.ViewModelExportWarningMessage);
                }
            }
        }
    }

    private async Task ImportUIAFFromClipboardAsync()
    {
        if (await achievementImporter.FromClipboardAsync().ConfigureAwait(false))
        {
            await UpdateAchievementsAsync(achievementService.CurrentArchive!).ConfigureAwait(false);
        }
    }

    private async Task ImportUIAFFromFileAsync()
    {
        if (await achievementImporter.FromFileAsync().ConfigureAwait(false))
        {
            await UpdateAchievementsAsync(achievementService.CurrentArchive!).ConfigureAwait(false);
        }
    }

    private async Task UpdateAchievementsAsync(EntityAchievementArchive archive)
    {
        List<MetadataAchievement> rawAchievements = await metadataService.GetAchievementsAsync(CancellationToken).ConfigureAwait(false);

        if (TryGetAchievements(archive, rawAchievements, out List<BindingAchievement>? combined))
        {
            // Assemble achievements on the UI thread.
            await ThreadHelper.SwitchToMainThreadAsync();
            Achievements = new(combined, true);

            UpdateAchievementsFinishPercent();
            UpdateAchievementsFilterByGoal(SelectedAchievementGoal);
            UpdateAchievementsSort();
        }
    }

    private bool TryGetAchievements(EntityAchievementArchive archive, List<MetadataAchievement> achievements, out List<BindingAchievement>? combined)
    {
        try
        {
            combined = achievementService.GetAchievements(archive, achievements);
            return true;
        }
        catch (Core.ExceptionService.UserdataCorruptedException ex)
        {
            combined = default;
            infoBarService.Error(ex);
            return false;
        }
    }

    private void UpdateAchievementsSort()
    {
        if (Achievements != null)
        {
            if (IsIncompletedItemsFirst)
            {
                Achievements.SortDescriptions.Add(IncompletedItemsFirstSortDescription);
                Achievements.SortDescriptions.Add(CompletionTimeSortDescription);
            }
            else
            {
                Achievements.SortDescriptions.Clear();
            }
        }
    }

    private void UpdateAchievementsFilterByGoal(BindingAchievementGoal? goal)
    {
        if (Achievements != null)
        {
            if (goal == null)
            {
                Achievements.Filter = null;
            }
            else
            {
                int goalId = goal.Id;
                Achievements.Filter = (object o) => o is BindingAchievement achi && achi.Inner.Goal == goalId;
            }
        }
    }

    private void UpdateAchievementsFilterBySerach(string? search)
    {
        if (Achievements != null)
        {
            SetProperty(ref selectedAchievementGoal, null);

            if (!string.IsNullOrEmpty(search))
            {
                if (search.Length == 5 && int.TryParse(search, out int achiId))
                {
                    Achievements.Filter = obj => ((BindingAchievement)obj).Inner.Id == achiId;
                }
                else
                {
                    Achievements.Filter = obj =>
                    {
                        BindingAchievement achi = (BindingAchievement)obj;
                        return achi.Inner.Title.Contains(search) || achi.Inner.Description.Contains(search);
                    };
                }
            }
        }
    }

    // 仅 读取成就列表 与 保存成就状态 时需要刷新成就进度
    private void UpdateAchievementsFinishPercent()
    {
        achievementFinishPercentUpdater.Update();
    }

    private void SaveAchievement(BindingAchievement? achievement)
    {
        if (achievement != null)
        {
            achievementService.SaveAchievement(achievement);
            UpdateAchievementsFinishPercent();
        }
    }
}