// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Windows.System;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;
using MetadataAchievementGoal = Snap.Hutao.Model.Metadata.Achievement.AchievementGoal;

namespace Snap.Hutao.ViewModel.Achievement;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class AchievementViewModel : Abstraction.ViewModel, INavigationRecipient, IDisposable
{
    public const string ImportUIAFFromClipboard = nameof(ImportUIAFFromClipboard);

    private readonly SortDescription achievementUncompletedItemsFirstSortDescription = new(nameof(AchievementView.IsChecked), SortDirection.Ascending);
    private readonly SortDescription achievementCompletionTimeSortDescription = new(nameof(AchievementView.Time), SortDirection.Descending);
    private readonly SortDescription achievementGoalUncompletedItemsFirstSortDescription = new(nameof(AchievementGoalView.FinishPercent), SortDirection.Ascending);

    private readonly SortDescription achievementDefaultSortDescription = new(nameof(AchievementView.Order), SortDirection.Ascending);
    private readonly SortDescription achievementGoalDefaultSortDescription = new(nameof(AchievementGoalView.Order), SortDirection.Ascending);

    private readonly ExclusiveTokenProvider achievementsTokenProvider = new();

    private readonly AchievementViewModelScopeContext scopeContext;

    [GeneratedConstructor]
    public partial AchievementViewModel(IServiceProvider serviceProvider);

    public IAdvancedDbCollectionView<EntityArchive>? Archives
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentArchiveChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(value, OnCurrentArchiveChanged);
        }
    }

    [ObservableProperty]
    public partial IAdvancedCollectionView<AchievementView>? Achievements { get; set; }

    public IAdvancedCollectionView<AchievementGoalView>? AchievementGoals
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentAchievementGoalChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(value, OnCurrentAchievementGoalChanged);
        }
    }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsUncompletedItemsFirst { get; set; } = true;

    [ObservableProperty]
    public partial bool FilterDailyQuestItems { get; set; }

    [ObservableProperty]
    public partial string? FinishDescription { get; set; }

    [GeneratedRegex("\\d\\.\\d")]
    private static partial Regex VersionRegex { get; }

    public async ValueTask<bool> ReceiveAsync(INavigationExtraData data, CancellationToken token)
    {
        if (!await Initialization.Task.ConfigureAwait(false))
        {
            return false;
        }

        if (data.Data is ImportUIAFFromClipboard)
        {
            await ImportUIAFFromClipboardAsync().ConfigureAwait(false);
            return true;
        }

        return false;
    }

    public override void Dispose()
    {
        achievementsTokenProvider.Dispose();
        base.Dispose();
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await scopeContext.MetadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        token.ThrowIfCancellationRequested();

        IAdvancedCollectionView<AchievementGoalView> sortedGoals;
        IAdvancedDbCollectionView<EntityArchive> archives;
        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            ImmutableArray<MetadataAchievementGoal> goals = await scopeContext.MetadataService
                .GetAchievementGoalArrayAsync(token)
                .ConfigureAwait(false);

            sortedGoals = goals.OrderBy(goal => goal.Order).Select(AchievementGoalView.Create).AsAdvancedCollectionView();
            archives = await scopeContext.AchievementService.GetArchiveCollectionAsync(token).ConfigureAwait(false);
        }

        await scopeContext.TaskContext.SwitchToMainThreadAsync();

        AchievementGoals = sortedGoals;
        Archives = archives;
        Archives.MoveCurrentTo(Archives.Source.SelectedOrFirstOrDefault());

        return true;
    }

    protected override void UninitializeOverride()
    {
        using (Archives?.SuppressChangeCurrentItem())
        {
            Archives = default;
        }

        AchievementGoals = default;
    }

    private void OnCurrentArchiveChanged(object? sender, object? e)
    {
        UpdateAchievementCollectionAsync(Archives?.CurrentItem, achievementsTokenProvider.GetNewToken()).SafeForget();
    }

    private void OnCurrentAchievementGoalChanged(object? sender, object? e)
    {
        SearchText = string.Empty;
        UpdateAchievementsFilterByGoal(AchievementGoals?.CurrentItem);
    }

    [Command("AddArchiveCommand")]
    private async Task AddArchiveAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Add archive", "AchievementViewModel.Command"));

        if (Archives is null)
        {
            return;
        }

        AchievementArchiveCreateDialog dialog = await scopeContext.ContentDialogFactory.CreateInstanceAsync<AchievementArchiveCreateDialog>(scopeContext.ServiceProvider).ConfigureAwait(false);
        if (await dialog.GetInputAsync().ConfigureAwait(false) is not (true, { } name))
        {
            return;
        }

        InfoBarMessage message = await scopeContext.AchievementService.AddArchiveAsync(EntityArchive.Create(name)).ConfigureAwait(false) switch
        {
            ArchiveAddResultKind.Added => InfoBarMessage.Success(SH.FormatViewModelAchievementArchiveAdded(name)),
            ArchiveAddResultKind.ArchiveNameInvalid => InfoBarMessage.Warning(SH.ViewModelAchievementArchiveInvalidName),
            ArchiveAddResultKind.ArchiveAlreadyExists => InfoBarMessage.Warning(SH.FormatViewModelAchievementArchiveAlreadyExists(name)),
            _ => throw HutaoException.NotSupported(),
        };

        scopeContext.Messenger.Send(message);
    }

    [Command("RemoveArchiveCommand")]
    private async Task RemoveArchiveAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove archive", "AchievementViewModel.Command"));

        if (Archives?.CurrentItem is not { } current)
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
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Command("ExportAsUIAFToFileCommand")]
    private async Task ExportAsUIAFToFileAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Export UIAF file", "AchievementViewModel.Command"));

        if (Archives?.CurrentItem is null || Achievements is null)
        {
            return;
        }

        FileSystemPickerOptions pickerOptions = new()
        {
            Title = SH.ViewModelAchievementUIAFExportPickerTitle,
            DefaultFileName = $"{Archives.CurrentItem.Name}.json",
            FilterName = SH.ViewModelAchievementExportFileType,
            FilterType = "*.json",
        };

        if (scopeContext.FileSystemPickerInteraction.SaveFile(pickerOptions) is not (true, { HasValue: true } file))
        {
            return;
        }

        UIAF uiaf = await scopeContext.AchievementService.ExportToUIAFAsync(Archives.CurrentItem).ConfigureAwait(false);

        InfoBarMessage message = await file.SerializeToJsonNoThrowAsync(uiaf, scopeContext.JsonSerializerOptions).ConfigureAwait(false)
            ? InfoBarMessage.Success(SH.ViewModelExportSuccessTitle, SH.ViewModelExportSuccessMessage)
            : InfoBarMessage.Warning(SH.ViewModelExportWarningTitle, SH.ViewModelExportWarningMessage);

        scopeContext.Messenger.Send(message);
    }

    [Command("ImportUIAFFromEmbeddedYaeCommand")]
    private async Task ImportUIAFFromEmbeddedYaeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Import UIAF", "AchievementViewModel.Command", [("source", "Embedded Yae")]));

        if (!HutaoRuntime.IsProcessElevated)
        {
            await scopeContext.ContentDialogFactory.CreateForConfirmAsync(SH.ViewModelYaeProcessNotElevatedTitle, SH.ViewModelYaeProcessNotElevatedDescription).ConfigureAwait(false);
            return;
        }

        if (await scopeContext.AchievementImporter.FromEmbeddedYaeAsync(scopeContext).ConfigureAwait(false))
        {
            await UpdateAchievementCollectionAsync(Archives?.CurrentItem, achievementsTokenProvider.GetNewToken()).ConfigureAwait(false);
        }
    }

    [Command("ImportUIAFFromClipboardCommand")]
    private async Task ImportUIAFFromClipboardAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Import UIAF", "AchievementViewModel.Command", [("source", "Clipboard")]));

        if (await scopeContext.AchievementImporter.FromClipboardAsync(scopeContext).ConfigureAwait(false))
        {
            await UpdateAchievementCollectionAsync(Archives?.CurrentItem, achievementsTokenProvider.GetNewToken()).ConfigureAwait(false);
        }
    }

    [Command("ImportUIAFFromFileCommand")]
    private async Task ImportUIAFFromFileAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Import UIAF", "AchievementViewModel.Command", [("source", "File")]));

        if (await scopeContext.AchievementImporter.FromFileAsync(scopeContext).ConfigureAwait(false))
        {
            await UpdateAchievementCollectionAsync(Archives?.CurrentItem, achievementsTokenProvider.GetNewToken()).ConfigureAwait(false);
        }
    }

    private async ValueTask UpdateAchievementCollectionAsync(EntityArchive? archive, CancellationToken token)
    {
        await scopeContext.TaskContext.InvokeOnMainThreadAsync(() => Achievements = default).ConfigureAwait(false);
        token.ThrowIfCancellationRequested();

        if (archive is null)
        {
            return;
        }

        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, CancellationToken);
        AchievementServiceMetadataContext context = await scopeContext.MetadataService
            .GetContextAsync<AchievementServiceMetadataContext>(linkedCts.Token)
            .ConfigureAwait(false);

        if (!TryGetAchievementViewCollection(archive, context, out IAdvancedCollectionView<AchievementView>? collection))
        {
            return;
        }

        await scopeContext.TaskContext.SwitchToMainThreadAsync();
        token.ThrowIfCancellationRequested();

        Achievements = collection;
        AchievementFinishPercent.Update(this);
        UpdateAchievementsFilterByGoal(AchievementGoals?.CurrentItem);
        UpdateAchievementsSort();
    }

    private bool TryGetAchievementViewCollection(EntityArchive archive, AchievementServiceMetadataContext context, [NotNullWhen(true)] out IAdvancedCollectionView<AchievementView>? view)
    {
        try
        {
            view = scopeContext.AchievementService.GetAchievementViewCollection(archive, context);
            return true;
        }
        catch (HutaoException ex)
        {
            scopeContext.Messenger.Send(InfoBarMessage.Error(ex));
            view = default;
            return false;
        }
    }

    [Command("SortUncompletedSwitchCommand")]
    private void UpdateAchievementsSort()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Sort by IsCompleted", "AchievementViewModel.Command", [("value", $"{IsUncompletedItemsFirst}")]));

        if (Achievements is null || AchievementGoals is null)
        {
            return;
        }

        using (Achievements.DeferRefresh())
        {
            using (AchievementGoals.DeferRefresh())
            {
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
        }
    }

    private void UpdateAchievementsFilterByGoal(AchievementGoalView? goal)
    {
        Achievements?.Filter = AchievementFilter.Compile(FilterDailyQuestItems, goal);
    }

    [Command("SearchAchievementCommand")]
    private void UpdateAchievementsFilterBySearch(string? search)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Search", "AchievementViewModel.Command", [("text", search ?? "<null>")]));

        if (Achievements is null || AchievementGoals is null)
        {
            return;
        }

        AchievementGoals.MoveCurrentTo(default);

        if (string.IsNullOrEmpty(search))
        {
            Achievements.Filter = AchievementFilter.Compile(FilterDailyQuestItems);
            AchievementGoals.Filter = AchievementFilter.GoalCompile(Achievements);
            return;
        }

        if (uint.TryParse(search, out uint achievementId))
        {
            Achievements.Filter = AchievementFilter.Compile(FilterDailyQuestItems, achievementId);
            AchievementGoals.Filter = AchievementFilter.GoalCompile(Achievements);
            return;
        }

        if (VersionRegex.IsMatch(search))
        {
            Achievements.Filter = AchievementFilter.CompileForVersion(FilterDailyQuestItems, search);
            AchievementGoals.Filter = AchievementFilter.GoalCompile(Achievements);
            return;
        }

        Achievements.Filter = AchievementFilter.CompileForTitleOrDescription(FilterDailyQuestItems, search);
        AchievementGoals.Filter = AchievementFilter.GoalCompile(Achievements);
    }

    [Command("SaveAchievementCommand")]
    private void SaveAchievement(AchievementView? achievement)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Save single achievement", "AchievementViewModel.Command"));

        if (achievement is null)
        {
            return;
        }

        scopeContext.AchievementService.SaveAchievement(achievement);
        AchievementFinishPercent.Update(this);
    }

    [Command("FilterDailyQuestSwitchCommand")]
    private void UpdateAchievementsFilterByDailyQuest()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Filter by IsDailyQuest", "AchievementViewModel.Command", [("value", $"{IsUncompletedItemsFirst}")]));

        if (Achievements is null || AchievementGoals is null)
        {
            return;
        }

        Achievements.Filter = AchievementFilter.Compile(FilterDailyQuestItems);
        AchievementGoals.Filter = AchievementFilter.GoalCompile(Achievements);
    }

    [Command("CopyAchievementIdCommand")]
    private async Task CopyAchievementIdAsync(AchievementView? achievement)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Copy achievement id", "AchievementViewModel.Command"));

        if (achievement is null)
        {
            return;
        }

        try
        {
            await scopeContext.ClipboardProvider.SetTextAsync(achievement.Inner.Id.ToString()).ConfigureAwait(false);
            scopeContext.Messenger.Send(InfoBarMessage.Success(SH.ViewModelAchievementCopyAchievementIdSuccess));
        }
        catch (COMException ex)
        {
            scopeContext.Messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("SearchInMiyousheCommand")]
    private async Task SearchInMiyousheAsync(AchievementView? achievement)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Search achievement in Miyoushe", "AchievementViewModel.Command"));

        if (achievement is null)
        {
            return;
        }

        string keyword = Uri.EscapeDataString(SH.FormatViewModelAchievementSearchQuery(achievement.Inner.Title));
        Uri targetUri = $"https://www.miyoushe.com/ys/search?keyword={keyword}".ToUri();
        await Launcher.LaunchUriAsync(targetUri);
    }

    [Command("SearchInHoYoLABCommand")]
    private async Task SearchInHoYoLABAsync(AchievementView? achievement)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Search achievement in HoYoLAB", "AchievementViewModel.Command"));

        if (achievement is null)
        {
            return;
        }

        string keyword = Uri.EscapeDataString(SH.FormatViewModelAchievementSearchQuery(achievement.Inner.Title));
        Uri targetUri = $"https://www.hoyolab.com/search?keyword={keyword}".ToUri();
        await Launcher.LaunchUriAsync(targetUri);
    }
}