// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;
using EntityAchievementArchive = Snap.Hutao.Model.Entity.AchievementArchive;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;
using MetadataAchievementGoal = Snap.Hutao.Model.Metadata.Achievement.AchievementGoal;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 成就视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementViewModel : Abstraction.ViewModel, INavigationRecipient
{
    private static readonly SortDescription UncompletedItemsFirstSortDescription = new(nameof(AchievementView.IsChecked), SortDirection.Ascending);
    private static readonly SortDescription CompletionTimeSortDescription = new(nameof(AchievementView.Time), SortDirection.Descending);

    private readonly TaskCompletionSource<bool> openUITaskCompletionSource;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly AchievementImporter achievementImporter;
    private readonly IAchievementService achievementService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;
    private readonly ITaskContext taskContext;

    private AdvancedCollectionView? achievements;
    private List<AchievementGoalView>? achievementGoals;
    private AchievementGoalView? selectedAchievementGoal;
    private ObservableCollection<EntityAchievementArchive>? archives;
    private EntityAchievementArchive? selectedArchive;
    private bool isUncompletedItemsFirst = true;
    private string searchText = string.Empty;
    private string? finishDescription;

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
                UpdateAchievementsAsync(value).SafeForget();
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
    public List<AchievementGoalView>? AchievementGoals
    {
        get => achievementGoals;
        set => SetProperty(ref achievementGoals, value);
    }

    /// <summary>
    /// 选中的成就分类
    /// </summary>
    public AchievementGoalView? SelectedAchievementGoal
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
    public bool IsUncompletedItemsFirst
    {
        get => isUncompletedItemsFirst;
        set => SetProperty(ref isUncompletedItemsFirst, value);
    }

    /// <summary>
    /// 完成进度描述
    /// </summary>
    public string? FinishDescription
    {
        get => finishDescription;
        set => SetProperty(ref finishDescription, value);
    }

    /// <inheritdoc/>
    public async Task<bool> ReceiveAsync(INavigationData data)
    {
        if (await openUITaskCompletionSource.Task.ConfigureAwait(false))
        {
            if (data.Data is Activation.ImportUIAFFromClipboard)
            {
                await ImportUIAFFromClipboardAsync().ConfigureAwait(false);
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        bool metaInitialized = await metadataService.InitializeAsync().ConfigureAwait(false);

        if (metaInitialized)
        {
            try
            {
                List<AchievementGoalView> sortedGoals;
                ObservableCollection<EntityAchievementArchive> archives;

                using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                {
                    List<MetadataAchievementGoal> goals = await metadataService
                        .GetAchievementGoalsAsync(CancellationToken)
                        .ConfigureAwait(false);

                    sortedGoals = AchievementGoalView.List(goals);
                    archives = achievementService.ArchiveCollection;
                }

                await taskContext.SwitchToMainThreadAsync();
                AchievementGoals = sortedGoals;
                Archives = archives;
                SelectedArchive = achievementService.CurrentArchive;

                IsInitialized = true;
            }
            catch (OperationCanceledException)
            {
                // User canceled the loading operation,
                // Indicate initialization not succeed.
                openUITaskCompletionSource.TrySetResult(false);
                return;
            }
        }

        openUITaskCompletionSource.TrySetResult(metaInitialized);
    }

    [Command("AddArchiveCommand")]
    private async Task AddArchiveAsync()
    {
        if (Archives != null)
        {
            // ContentDialog must be created by main thread.
            await taskContext.SwitchToMainThreadAsync();
            AchievementArchiveCreateDialog dialog = serviceProvider.CreateInstance<AchievementArchiveCreateDialog>();
            (bool isOk, string name) = await dialog.GetInputAsync().ConfigureAwait(false);

            if (isOk)
            {
                ArchiveAddResult result = await achievementService.TryAddArchiveAsync(EntityAchievementArchive.Create(name)).ConfigureAwait(false);

                switch (result)
                {
                    case ArchiveAddResult.Added:
                        await taskContext.SwitchToMainThreadAsync();
                        SelectedArchive = achievementService.CurrentArchive;
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

    [Command("RemoveArchiveCommand")]
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
                    await taskContext.SwitchToMainThreadAsync();
                    SelectedArchive = Archives.FirstOrDefault();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }

    [Command("ExportAsUIAFToFileCommand")]
    private async Task ExportAsUIAFToFileAsync()
    {
        if (SelectedArchive != null && Achievements != null)
        {
            Dictionary<string, IList<string>> fileTypes = new()
            {
                [SH.ViewModelAchievementExportFileType] = ".json".Enumerate().ToList(),
            };

            FileSavePicker picker = serviceProvider
                .GetRequiredService<IPickerFactory>()
                .GetFileSavePicker(
                    PickerLocationId.Desktop,
                    $"{achievementService.CurrentArchive?.Name}.json",
                    SH.FilePickerExportCommit,
                    fileTypes);

            (bool isPickerOk, ValueFile file) = await picker.TryPickSaveFileAsync().ConfigureAwait(false);
            if (isPickerOk)
            {
                UIAF uiaf = await achievementService.ExportToUIAFAsync(SelectedArchive).ConfigureAwait(false);
                if (await file.SerializeToJsonAsync(uiaf, options).ConfigureAwait(false))
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

    [Command("ImportUIAFFromClipboardCommand")]
    private async Task ImportUIAFFromClipboardAsync()
    {
        if (await achievementImporter.FromClipboardAsync().ConfigureAwait(false))
        {
            await UpdateAchievementsAsync(achievementService.CurrentArchive!).ConfigureAwait(false);
        }
    }

    [Command("ImportUIAFFromFileCommand")]
    private async Task ImportUIAFFromFileAsync()
    {
        if (await achievementImporter.FromFileAsync().ConfigureAwait(false))
        {
            await UpdateAchievementsAsync(achievementService.CurrentArchive!).ConfigureAwait(false);
        }
    }

    private async Task UpdateAchievementsAsync(EntityAchievementArchive? archive)
    {
        if (archive == null)
        {
            return;
        }

        List<MetadataAchievement> achievements = await metadataService.GetAchievementsAsync(CancellationToken).ConfigureAwait(false);

        if (TryGetAchievements(archive, achievements, out List<AchievementView>? combined))
        {
            await taskContext.SwitchToMainThreadAsync();
            Achievements = new(combined, true); // Assemble achievements on the UI thread.

            UpdateAchievementsFinishPercent();
            UpdateAchievementsFilterByGoal(SelectedAchievementGoal);
            UpdateAchievementsSort();
        }
    }

    private bool TryGetAchievements(EntityAchievementArchive archive, List<MetadataAchievement> achievements, out List<AchievementView>? combined)
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

    [Command("SortUncompletedSwitchCommand")]
    private void UpdateAchievementsSort()
    {
        if (Achievements != null)
        {
            if (IsUncompletedItemsFirst)
            {
                Achievements.SortDescriptions.Add(UncompletedItemsFirstSortDescription);
                Achievements.SortDescriptions.Add(CompletionTimeSortDescription);
            }
            else
            {
                Achievements.SortDescriptions.Clear();
            }
        }
    }

    private void UpdateAchievementsFilterByGoal(AchievementGoalView? goal)
    {
        if (Achievements != null)
        {
            if (goal == null)
            {
                Achievements.Filter = null;
            }
            else
            {
                Achievements.Filter = (object o) => o is AchievementView view && view.Inner.Goal == goal.Id;
            }
        }
    }

    [Command("SearchAchievementCommand")]
    private void UpdateAchievementsFilterBySearch(string? search)
    {
        if (Achievements != null)
        {
            SetProperty(ref selectedAchievementGoal, null);

            if (!string.IsNullOrEmpty(search))
            {
                if (search.Length == 5 && int.TryParse(search, out int achievementId))
                {
                    Achievements.Filter = obj => ((AchievementView)obj).Inner.Id.Value == achievementId;
                }
                else
                {
                    Achievements.Filter = obj =>
                    {
                        AchievementView view = (AchievementView)obj;
                        return view.Inner.Title.Contains(search) || view.Inner.Description.Contains(search);
                    };
                }
            }
        }
    }

    private void UpdateAchievementsFinishPercent()
    {
        // 仅 读取成就列表 与 保存成就状态 时需要刷新成就进度
        AchievementFinishPercent.Update(this);
    }

    [Command("SaveAchievementCommand")]
    private void SaveAchievement(AchievementView? achievement)
    {
        if (achievement != null)
        {
            achievementService.SaveAchievement(achievement);
            UpdateAchievementsFinishPercent();
        }
    }
}