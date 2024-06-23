// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using EntityAchievementArchive = Snap.Hutao.Model.Entity.AchievementArchive;
using MetadataAchievementGoal = Snap.Hutao.Model.Metadata.Achievement.AchievementGoal;
using SortDescription = CommunityToolkit.WinUI.Collections.SortDescription;
using SortDirection = CommunityToolkit.WinUI.Collections.SortDirection;

namespace Snap.Hutao.ViewModel.Achievement;

[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementViewModel : Abstraction.ViewModel, INavigationRecipient
{
    private readonly SortDescription achievementUncompletedItemsFirstSortDescription = new(nameof(AchievementView.IsChecked), SortDirection.Ascending);
    private readonly SortDescription achievementCompletionTimeSortDescription = new(nameof(AchievementView.Time), SortDirection.Descending);
    private readonly SortDescription achievementGoalUncompletedItemsFirstSortDescription = new(nameof(AchievementGoalView.FinishPercent), SortDirection.Ascending);

    private readonly SortDescription achievementDefaultSortDescription = new(nameof(AchievementView.Order), SortDirection.Ascending);
    private readonly SortDescription achievementGoalDefaultSortDescription = new(nameof(AchievementGoalView.Order), SortDirection.Ascending);

    private readonly AchievementViewModelDependencies dependencies;

    private AdvancedCollectionView<AchievementView>? achievements;
    private AdvancedCollectionView<AchievementGoalView>? achievementGoals;
    private AchievementGoalView? selectedAchievementGoal;
    private ObservableCollection<EntityAchievementArchive>? archives;
    private EntityAchievementArchive? selectedArchive;
    private bool isUncompletedItemsFirst = true;
    private string searchText = string.Empty;
    private string? finishDescription;

    public ObservableCollection<EntityAchievementArchive>? Archives
    {
        get => archives;
        set => SetProperty(ref archives, value);
    }

    public EntityAchievementArchive? SelectedArchive
    {
        get => selectedArchive;
        set
        {
            if (SetProperty(ref selectedArchive, value))
            {
                dependencies.AchievementService.CurrentArchive = value;
                UpdateAchievementsAsync(value).SafeForget();
            }
        }
    }

    public AdvancedCollectionView<AchievementView>? Achievements
    {
        get => achievements;
        set => SetProperty(ref achievements, value);
    }

    public AdvancedCollectionView<AchievementGoalView>? AchievementGoals
    {
        get => achievementGoals;
        set => SetProperty(ref achievementGoals, value);
    }

    public AchievementGoalView? SelectedAchievementGoal
    {
        get => selectedAchievementGoal;
        set
        {
            if (SetProperty(ref selectedAchievementGoal, value))
            {
                SearchText = string.Empty;
                UpdateAchievementsFilterByGoal(value);
            }
        }
    }

    public string SearchText
    {
        get => searchText;
        set => SetProperty(ref searchText, value);
    }

    public bool IsUncompletedItemsFirst
    {
        get => isUncompletedItemsFirst;
        set => SetProperty(ref isUncompletedItemsFirst, value);
    }

    public string? FinishDescription
    {
        get => finishDescription;
        set => SetProperty(ref finishDescription, value);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> ReceiveAsync(INavigationData data)
    {
        if (await Initialization.Task.ConfigureAwait(false))
        {
            if (data.Data is AppActivation.ImportUIAFFromClipboard)
            {
                await ImportUIAFFromClipboardAsync().ConfigureAwait(false);
                return true;
            }
        }

        return false;
    }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        if (!await dependencies.MetadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        List<AchievementGoalView> sortedGoals;
        ObservableCollection<EntityAchievementArchive> archives;

        using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
        {
            List<MetadataAchievementGoal> goals = await dependencies.MetadataService
                .GetAchievementGoalListAsync(CancellationToken)
                .ConfigureAwait(false);

            sortedGoals = goals.SortBy(goal => goal.Order).SelectList(AchievementGoalView.From);
            archives = dependencies.AchievementService.ArchiveCollection;
        }

        await dependencies.TaskContext.SwitchToMainThreadAsync();

        AchievementGoals = new(sortedGoals, true);
        Archives = archives;
        SelectedArchive = dependencies.AchievementService.CurrentArchive;
        return true;
    }

    [GeneratedRegex("\\d\\.\\d")]
    private static partial Regex VersionRegex();

    [Command("AddArchiveCommand")]
    private async Task AddArchiveAsync()
    {
        if (Archives is not null)
        {
            AchievementArchiveCreateDialog dialog = await dependencies.ContentDialogFactory.CreateInstanceAsync<AchievementArchiveCreateDialog>().ConfigureAwait(false);
            (bool isOk, string name) = await dialog.GetInputAsync().ConfigureAwait(false);

            if (isOk)
            {
                ArchiveAddResultKind result = await dependencies.AchievementService.AddArchiveAsync(EntityAchievementArchive.From(name)).ConfigureAwait(false);

                switch (result)
                {
                    case ArchiveAddResultKind.Added:
                        await dependencies.TaskContext.SwitchToMainThreadAsync();
                        SelectedArchive = dependencies.AchievementService.CurrentArchive;
                        dependencies.InfoBarService.Success(SH.FormatViewModelAchievementArchiveAdded(name));
                        break;
                    case ArchiveAddResultKind.InvalidName:
                        dependencies.InfoBarService.Warning(SH.ViewModelAchievementArchiveInvalidName);
                        break;
                    case ArchiveAddResultKind.AlreadyExists:
                        dependencies.InfoBarService.Warning(SH.FormatViewModelAchievementArchiveAlreadyExists(name));
                        break;
                    default:
                        throw HutaoException.NotSupported();
                }
            }
        }
    }

    [Command("RemoveArchiveCommand")]
    private async Task RemoveArchiveAsync()
    {
        if (Archives is not null && SelectedArchive is not null)
        {
            string title = SH.FormatViewModelAchievementRemoveArchiveTitle(SelectedArchive.Name);
            string content = SH.ViewModelAchievementRemoveArchiveContent;
            ContentDialogResult result = await dependencies.ContentDialogFactory
                .CreateForConfirmCancelAsync(title, content)
                .ConfigureAwait(false);

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                    {
                        await dependencies.AchievementService.RemoveArchiveAsync(SelectedArchive).ConfigureAwait(false);
                    }

                    // Re-select first archive
                    await dependencies.TaskContext.SwitchToMainThreadAsync();
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
        if (SelectedArchive is not null && Achievements is not null)
        {
            (bool isOk, ValueFile file) = dependencies.FileSystemPickerInteraction.SaveFile(
                SH.ViewModelAchievementUIAFExportPickerTitle,
                $"{dependencies.AchievementService.CurrentArchive?.Name}.json",
                [(SH.ViewModelAchievementExportFileType, "*.json")]);

            if (isOk)
            {
                UIAF uiaf = await dependencies.AchievementService.ExportToUIAFAsync(SelectedArchive).ConfigureAwait(false);
                if (await file.SerializeToJsonAsync(uiaf, dependencies.JsonSerializerOptions).ConfigureAwait(false))
                {
                    dependencies.InfoBarService.Success(SH.ViewModelExportSuccessTitle, SH.ViewModelExportSuccessMessage);
                }
                else
                {
                    dependencies.InfoBarService.Warning(SH.ViewModelExportWarningTitle, SH.ViewModelExportWarningMessage);
                }
            }
        }
    }

    [Command("ImportUIAFFromClipboardCommand")]
    private async Task ImportUIAFFromClipboardAsync()
    {
        if (await dependencies.AchievementImporter.FromClipboardAsync().ConfigureAwait(false))
        {
            ArgumentNullException.ThrowIfNull(dependencies.AchievementService.CurrentArchive);
            await UpdateAchievementsAsync(dependencies.AchievementService.CurrentArchive).ConfigureAwait(false);
        }
    }

    [Command("ImportUIAFFromFileCommand")]
    private async Task ImportUIAFFromFileAsync()
    {
        if (await dependencies.AchievementImporter.FromFileAsync().ConfigureAwait(false))
        {
            ArgumentNullException.ThrowIfNull(dependencies.AchievementService.CurrentArchive);
            await UpdateAchievementsAsync(dependencies.AchievementService.CurrentArchive).ConfigureAwait(false);
        }
    }

    private async ValueTask UpdateAchievementsAsync(EntityAchievementArchive? archive)
    {
        // TODO: immediately clear values
        if (archive is null)
        {
            return;
        }

        AchievementServiceMetadataContext context = await dependencies.MetadataService
            .GetContextAsync<AchievementServiceMetadataContext>(CancellationToken)
            .ConfigureAwait(false);

        if (TryGetAchievements(archive, context, out List<AchievementView>? combined))
        {
            await dependencies.TaskContext.SwitchToMainThreadAsync();

            Achievements = new(combined, true);
            UpdateAchievementsFinishPercent();
            UpdateAchievementsFilterByGoal(SelectedAchievementGoal);
            UpdateAchievementsSort();
        }
    }

    private bool TryGetAchievements(EntityAchievementArchive archive, AchievementServiceMetadataContext context, [NotNullWhen(true)] out List<AchievementView>? combined)
    {
        try
        {
            combined = dependencies.AchievementService.GetAchievementViewList(archive, context);
            return true;
        }
        catch (HutaoException ex)
        {
            dependencies.InfoBarService.Error(ex);
            combined = default;
            return false;
        }
    }

    [Command("SortUncompletedSwitchCommand")]
    private void UpdateAchievementsSort()
    {
        if (Achievements is null || AchievementGoals is null)
        {
            return;
        }

        Achievements.SortDescriptions.Clear();
        AchievementGoals.SortDescriptions.Clear();

        if (IsUncompletedItemsFirst)
        {
            Achievements.SortDescriptions.Add(achievementUncompletedItemsFirstSortDescription);
            Achievements.SortDescriptions.Add(achievementCompletionTimeSortDescription);
            AchievementGoals.SortDescriptions.Add(achievementGoalUncompletedItemsFirstSortDescription);
        }

        Achievements.SortDescriptions.Add(achievementDefaultSortDescription);
        AchievementGoals.SortDescriptions.Add(achievementGoalDefaultSortDescription);
    }

    private void UpdateAchievementsFilterByGoal(AchievementGoalView? goal)
    {
        if (Achievements is not null)
        {
            if (goal is null)
            {
                Achievements.Filter = default!;
            }
            else
            {
                Model.Primitive.AchievementGoalId goalId = goal.Id;
                Achievements.Filter = (AchievementView view) => view.Inner.Goal == goalId;
            }
        }
    }

    [Command("SearchAchievementCommand")]
    private void UpdateAchievementsFilterBySearch(string? search)
    {
        if (Achievements is not null)
        {
            SetProperty(ref selectedAchievementGoal, null);

            if (string.IsNullOrEmpty(search))
            {
                Achievements.Filter = default!;
                return;
            }

            if (uint.TryParse(search, out uint achievementId))
            {
                Achievements.Filter = view => view.Inner.Id == achievementId;
                return;
            }

            if (VersionRegex().IsMatch(search))
            {
                Achievements.Filter = view => view.Inner.Version == search;
                return;
            }

            Achievements.Filter = view =>
            {
                return view.Inner.Title.Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || view.Inner.Description.Contains(search, StringComparison.CurrentCultureIgnoreCase);
            };
        }
    }

    private void UpdateAchievementsFinishPercent()
    {
        // 保存成就状态时，需要保持当前选择的成就分类
        AchievementGoalView? currentSelectedAchievementGoal = SelectedAchievementGoal;

        // 仅 读取成就列表 与 保存成就状态 时需要刷新成就进度
        AchievementFinishPercent.Update(this);

        SelectedAchievementGoal = currentSelectedAchievementGoal;
    }

    [Command("SaveAchievementCommand")]
    private void SaveAchievement(AchievementView? achievement)
    {
        if (achievement is not null)
        {
            dependencies.AchievementService.SaveAchievement(achievement);
            UpdateAchievementsFinishPercent();
        }
    }
}