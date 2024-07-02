// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Database;
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
using Snap.Hutao.UI.Xaml.View.Dialog;
using System.Text.RegularExpressions;
using EntityAchievementArchive = Snap.Hutao.Model.Entity.AchievementArchive;
using MetadataAchievementGoal = Snap.Hutao.Model.Metadata.Achievement.AchievementGoal;

namespace Snap.Hutao.ViewModel.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementViewModel : Abstraction.ViewModel, INavigationRecipient
{
    private readonly SortDescription achievementUncompletedItemsFirstSortDescription = new(nameof(AchievementView.IsChecked), SortDirection.Ascending);
    private readonly SortDescription achievementCompletionTimeSortDescription = new(nameof(AchievementView.Time), SortDirection.Descending);
    private readonly SortDescription achievementGoalUncompletedItemsFirstSortDescription = new(nameof(AchievementGoalView.FinishPercent), SortDirection.Ascending);

    private readonly SortDescription achievementDefaultSortDescription = new(nameof(AchievementView.Order), SortDirection.Ascending);
    private readonly SortDescription achievementGoalDefaultSortDescription = new(nameof(AchievementGoalView.Order), SortDirection.Ascending);

    private readonly AchievementViewModelScopeContext scopeContext;

    private AdvancedCollectionView<AchievementView>? achievements;
    private AdvancedCollectionView<AchievementGoalView>? achievementGoals;
    private AdvancedDbCollectionView<EntityAchievementArchive>? archives;

    private bool isUncompletedItemsFirst = true;
    private string searchText = string.Empty;
    private string? finishDescription;

    public AdvancedDbCollectionView<EntityAchievementArchive>? Archives
    {
        get => archives;
        set
        {
            if (archives is not null)
            {
                archives.CurrentChanged -= OnCurrentArchiveChanged;
            }

            SetProperty(ref archives, value);

            if (value is not null)
            {
                value.CurrentChanged += OnCurrentArchiveChanged;
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
        set
        {
            if (achievementGoals is not null)
            {
                achievementGoals.CurrentChanged -= OnCurrentAchievementGoalChanged;
            }

            SetProperty(ref achievementGoals, value);

            if (value is not null)
            {
                value.CurrentChanged += OnCurrentAchievementGoalChanged;
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

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        if (!await scopeContext.MetadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        List<AchievementGoalView> sortedGoals;

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            List<MetadataAchievementGoal> goals = await scopeContext.MetadataService
                .GetAchievementGoalListAsync(CancellationToken)
                .ConfigureAwait(false);

            sortedGoals = goals.SortBy(goal => goal.Order).SelectList(AchievementGoalView.From);
        }

        await scopeContext.TaskContext.SwitchToMainThreadAsync();

        AchievementGoals = new(sortedGoals, true);
        Archives = scopeContext.AchievementService.Archives;
        Archives.MoveCurrentTo(Archives.SourceCollection.SelectedOrDefault());
        return true;
    }

    protected override void UninitializeOverride()
    {
        Archives?.Detach();
        Archives = default;
        AchievementGoals = default;
        Achievements = default;
    }

    [GeneratedRegex("\\d\\.\\d")]
    private static partial Regex VersionRegex();

    private void OnCurrentArchiveChanged(object? sender, object? e)
    {
        UpdateAchievementsAsync(Archives?.CurrentItem).SafeForget(scopeContext.Logger);
    }

    private void OnCurrentAchievementGoalChanged(object? sender, object? e)
    {
        SearchText = string.Empty;
        UpdateAchievementsFilterByGoal(AchievementGoals?.CurrentItem);
    }

    [Command("AddArchiveCommand")]
    private async Task AddArchiveAsync()
    {
        if (Archives is null)
        {
            return;
        }

        AchievementArchiveCreateDialog dialog = await scopeContext.ContentDialogFactory.CreateInstanceAsync<AchievementArchiveCreateDialog>().ConfigureAwait(false);
        (bool isOk, string name) = await dialog.GetInputAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        switch (await scopeContext.AchievementService.AddArchiveAsync(EntityAchievementArchive.From(name)).ConfigureAwait(false))
        {
            case ArchiveAddResultKind.Added:
                await scopeContext.TaskContext.SwitchToMainThreadAsync();
                scopeContext.InfoBarService.Success(SH.FormatViewModelAchievementArchiveAdded(name));
                break;
            case ArchiveAddResultKind.InvalidName:
                scopeContext.InfoBarService.Warning(SH.ViewModelAchievementArchiveInvalidName);
                break;
            case ArchiveAddResultKind.AlreadyExists:
                scopeContext.InfoBarService.Warning(SH.FormatViewModelAchievementArchiveAlreadyExists(name));
                break;
            default:
                throw HutaoException.NotSupported();
        }
    }

    [Command("RemoveArchiveCommand")]
    private async Task RemoveArchiveAsync()
    {
        if (Archives is null || !(Archives.CurrentItem is { } current))
        {
            return;
        }

        ContentDialogResult result = await scopeContext.ContentDialogFactory
            .CreateForConfirmCancelAsync(
                SH.FormatViewModelAchievementRemoveArchiveTitle(current.Name),
                SH.ViewModelAchievementRemoveArchiveContent)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        try
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                await scopeContext.AchievementService.RemoveArchiveAsync(current).ConfigureAwait(false);
            }

            await scopeContext.TaskContext.SwitchToMainThreadAsync();
            Archives.MoveCurrentToFirstOrDefault();
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Command("ExportAsUIAFToFileCommand")]
    private async Task ExportAsUIAFToFileAsync()
    {
        if (Archives?.CurrentItem is null || Achievements is null)
        {
            return;
        }

        (bool isOk, ValueFile file) = scopeContext.FileSystemPickerInteraction.SaveFile(
            SH.ViewModelAchievementUIAFExportPickerTitle,
            $"{Archives.CurrentItem.Name}.json",
            [(SH.ViewModelAchievementExportFileType, "*.json")]);

        if (!isOk)
        {
            return;
        }

        UIAF uiaf = await scopeContext.AchievementService.ExportToUIAFAsync(Archives.CurrentItem).ConfigureAwait(false);
        if (await file.SerializeToJsonAsync(uiaf, scopeContext.JsonSerializerOptions).ConfigureAwait(false))
        {
            scopeContext.InfoBarService.Success(SH.ViewModelExportSuccessTitle, SH.ViewModelExportSuccessMessage);
        }
        else
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelExportWarningTitle, SH.ViewModelExportWarningMessage);
        }
    }

    [Command("ImportUIAFFromClipboardCommand")]
    private async Task ImportUIAFFromClipboardAsync()
    {
        if (await scopeContext.AchievementImporter.FromClipboardAsync(scopeContext).ConfigureAwait(false))
        {
            await UpdateAchievementsAsync(Archives?.CurrentItem).ConfigureAwait(false);
        }
    }

    [Command("ImportUIAFFromFileCommand")]
    private async Task ImportUIAFFromFileAsync()
    {
        if (await scopeContext.AchievementImporter.FromFileAsync(scopeContext).ConfigureAwait(false))
        {
            await UpdateAchievementsAsync(Archives?.CurrentItem).ConfigureAwait(false);
        }
    }

    private async ValueTask UpdateAchievementsAsync(EntityAchievementArchive? archive)
    {
        // TODO: immediately clear values
        if (archive is null)
        {
            return;
        }

        AchievementServiceMetadataContext context = await scopeContext.MetadataService
            .GetContextAsync<AchievementServiceMetadataContext>(CancellationToken)
            .ConfigureAwait(false);

        if (!TryGetAchievements(archive, context, out List<AchievementView>? combined))
        {
            return;
        }

        await scopeContext.TaskContext.SwitchToMainThreadAsync();
        Achievements = new(combined, true);
        AchievementFinishPercent.Update(this);
        UpdateAchievementsFilterByGoal(AchievementGoals?.CurrentItem);
        UpdateAchievementsSort();
    }

    private bool TryGetAchievements(EntityAchievementArchive archive, AchievementServiceMetadataContext context, [NotNullWhen(true)] out List<AchievementView>? combined)
    {
        try
        {
            combined = scopeContext.AchievementService.GetAchievementViewList(archive, context);
            return true;
        }
        catch (HutaoException ex)
        {
            scopeContext.InfoBarService.Error(ex);
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
        if (Achievements is null)
        {
            return;
        }

        AchievementGoals?.MoveCurrentTo(default);

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

    [Command("SaveAchievementCommand")]
    private void SaveAchievement(AchievementView? achievement)
    {
        if (achievement is not null)
        {
            scopeContext.AchievementService.SaveAchievement(achievement);
            AchievementFinishPercent.Update(this);
        }
    }
}