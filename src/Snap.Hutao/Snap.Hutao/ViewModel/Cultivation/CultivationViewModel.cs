// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Inventory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.Yae;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.ViewModel.Game;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Cultivation;

[SuppressMessage("", "CA1001")]
[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class CultivationViewModel : Abstraction.ViewModel
{
    private readonly ExclusiveTokenProvider exclusiveTokenProvider = new();

    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ICultivationService cultivationService;
    private readonly INavigationService navigationService;
    private readonly IInventoryService inventoryService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IYaeService yaeService;
    private readonly IMessenger messenger;

    private CultivationMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial CultivationViewModel(IServiceProvider serviceProvider);

    public IAdvancedDbCollectionView<CultivateProject>? Projects
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentProjectChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentProjectChanged);
        }
    }

    [ObservableProperty]
    public partial ImmutableArray<InventoryItemView> InventoryItems { get; set; } = [];

    [ObservableProperty]
    public partial IAdvancedCollectionView<CultivateEntryView>? CultivateEntries { get; set; }

    [ObservableProperty]
    public partial bool EntriesUpdating { get; set; }

    [ObservableProperty]
    public partial bool IncompleteFirst { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<StatisticsCultivateItem>? StatisticsItems { get; set; }

    [ObservableProperty]
    public partial ResinStatistics? ResinStatistics { get; set; }

    [ObservableProperty]
    public partial SearchData? SearchData { get; set; }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        metadataContext = await metadataService.GetContextAsync<CultivationMetadataContext>(token).ConfigureAwait(false);
        SearchData searchData = SearchData.CreateForCultivation();

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            IAdvancedDbCollectionView<CultivateProject> projects = await cultivationService.GetProjectCollectionAsync().ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Projects = projects;
            Projects.MoveCurrentTo(Projects.Source.SelectedOrFirstOrDefault());
        }

        // Force update when re-entering the page
        if (Projects.CurrentItem is not null && CultivateEntries is null)
        {
            await UpdateEntryCollectionAsync(Projects.CurrentItem).ConfigureAwait(false);
        }

        await taskContext.SwitchToMainThreadAsync();
        SearchData = searchData;

        return true;
    }

    protected override void UninitializeOverride()
    {
        using (Projects?.SuppressChangeCurrentItem())
        {
            Projects = default;
        }
    }

    private void OnCurrentProjectChanged(object? sender, object? e)
    {
        UpdateEntryCollectionAsync(Projects?.CurrentItem).SafeForget();
    }

    [Command("AddProjectCommand")]
    private async Task AddProjectAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Add project", "CultivationViewModel.Command"));

        CultivateProjectDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivateProjectDialog>(serviceProvider).ConfigureAwait(false);
        (bool isOk, CultivateProject project) = await dialog.CreateProjectAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        InfoBarMessage message = await cultivationService.TryAddProjectAsync(project).ConfigureAwait(false) switch
        {
            ProjectAddResultKind.Added => InfoBarMessage.Success(SH.ViewModelCultivationProjectAdded),
            ProjectAddResultKind.InvalidName => InfoBarMessage.Information(SH.ViewModelCultivationProjectInvalidName),
            ProjectAddResultKind.AlreadyExists => InfoBarMessage.Information(SH.ViewModelCultivationProjectAlreadyExists),
            _ => throw HutaoException.NotSupported(),
        };

        messenger.Send(message);
    }

    [Command("RemoveProjectCommand")]
    private async Task RemoveProjectAsync(CultivateProject? project)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove project", "CultivationViewModel.Command"));

        if (project is null)
        {
            return;
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(
                SH.ViewModelCultivationRemoveProjectTitle,
                SH.ViewModelCultivationRemoveProjectContent)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        await cultivationService.RemoveProjectAsync(project).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        Projects?.MoveCurrentToFirst();
    }

    private async ValueTask UpdateEntryCollectionAsync(CultivateProject? project)
    {
        try
        {
            EntriesUpdating = true;
            if (project is null)
            {
                return;
            }

            CultivationMetadataContext context = await metadataService.GetContextAsync<CultivationMetadataContext>().ConfigureAwait(false);

            ObservableCollection<CultivateEntryView> entries = await cultivationService
                .GetCultivateEntryCollectionAsync(project, context)
                .ConfigureAwait(false);

            IAdvancedCollectionView<CultivateEntryView> entriesView = entries.AsAdvancedCollectionView();

            await taskContext.SwitchToMainThreadAsync();
            CultivateEntries = entriesView;

            await UpdateInventoryItemsAsync().ConfigureAwait(false);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
        finally
        {
            await taskContext.SwitchToMainThreadAsync();
            EntriesUpdating = false;
        }
    }

    [Command("RemoveEntryCommand")]
    private async Task RemoveEntryAsync(CultivateEntryView? entry)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove entry", "CultivationViewModel.Command"));

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
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Toggle item finish state", "CultivationViewModel.Command"));

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
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Save inventory item", "CultivationViewModel.Command"));

        if (inventoryItem is not null)
        {
            inventoryService.SaveInventoryItem(inventoryItem);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("RefreshInventoryByEmbeddedYaeCommand")]
    private async Task RefreshInventoryByEmbeddedYaeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Refresh inventory", "CultivationViewModel.Command", [("source", "Embedded Yae")]));

        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        if (!HutaoRuntime.IsProcessElevated)
        {
            await contentDialogFactory.CreateForConfirmAsync(SH.ViewModelYaeProcessNotElevatedTitle, SH.ViewModelYaeProcessNotElevatedDescription).ConfigureAwait(false);
            return;
        }

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            EmbeddedYaeLaunchExecutionViewModel viewModel = serviceProvider.GetRequiredService<EmbeddedYaeLaunchExecutionViewModel>();
            if (!await viewModel.InitializeAsync().ConfigureAwait(false))
            {
                return;
            }

            RefreshOptions options = RefreshOptions.CreateForEmbeddedYae(Projects.CurrentItem, yaeService, viewModel);
            await inventoryService.RefreshInventoryAsync(options).ConfigureAwait(false);

            await UpdateInventoryItemsAsync().ConfigureAwait(false);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("RefreshInventoryByCalculatorCommand")]
    private async Task RefreshInventoryByCalculatorAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Refresh inventory", "CultivationViewModel.Command", [("source", "Web Calculator")]));

        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            ContentDialog dialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.ViewModelCultivationRefreshInventoryProgress)
                .ConfigureAwait(false);

            using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
            {
                await inventoryService.RefreshInventoryAsync(RefreshOptions.CreateForWebCalculator(Projects.CurrentItem, metadataContext)).ConfigureAwait(false);

                await UpdateInventoryItemsAsync().ConfigureAwait(false);
                await UpdateStatisticsItemsAsync().ConfigureAwait(false);
            }
        }
    }

    [Command("ClearInventoryCommand")]
    private async Task ClearInventoryAsync(CultivateProject? project)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Clear inventory", "CultivationViewModel.Command"));

        if (project is null)
        {
            return;
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(
                SH.ViewModelCultivationClearInventoryTitle,
                SH.ViewModelCultivationClearInventoryContent)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelCultivationClearInventoryProgress)
            .ConfigureAwait(false);
        using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            inventoryService.RemoveInventoryItems(project);

            await UpdateInventoryItemsAsync().ConfigureAwait(false);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("RefreshStatisticsItemsCommand")]
    private async Task UpdateStatisticsItemsAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Refresh statistics items", "CultivationViewModel.Command"));

        // https://github.com/DGP-Studio/Snap.Hutao/issues/2044
        // Force clear the list and bring view to the top to prevent UI dead loop
        await taskContext.SwitchToMainThreadAsync();
        StatisticsItems = null;
        ResinStatistics = null;

        if (Projects?.CurrentItem is null)
        {
            return;
        }

        if (metadataContext is null)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();

        CancellationToken token = exclusiveTokenProvider.GetNewToken();
        StatisticsCultivateItemCollection statistics;
        ResinStatistics resinStatistics;
        try
        {
            statistics = await cultivationService.GetStatisticsCultivateItemCollectionAsync(Projects.CurrentItem, metadataContext, token).ConfigureAwait(false);
            resinStatistics = await cultivationService.GetResinStatisticsAsync(statistics, token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (IncompleteFirst)
        {
            statistics.SortAsIncompleteFirst();
        }

        await taskContext.SwitchToMainThreadAsync();
        StatisticsItems = statistics.ToObservableCollection();
        ResinStatistics = resinStatistics;
    }

    private async ValueTask UpdateInventoryItemsAsync()
    {
        // https://github.com/DGP-Studio/Snap.Hutao/issues/2044
        // Force clear the list and bring view to the top to prevent UI dead loop
        await taskContext.SwitchToMainThreadAsync();
        InventoryItems = [];

        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        InventoryItems = inventoryService.GetInventoryItemViews(metadataContext, Projects.CurrentItem, SaveInventoryItemCommand);
    }

    [Command("NavigateToPageCommand")]
    private void NavigateToPage(string? typeString)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI($"Navigate to {typeString}", "CultivationViewModel.Command"));

        if (typeString is not null)
        {
            Type? pageType = Type.GetType(typeString);
            ArgumentNullException.ThrowIfNull(pageType);
            navigationService.Navigate(pageType, NavigationExtraData.Default, true);
        }
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Filter", "CultivationViewModel.Command"));

        if (CultivateEntries is null || metadataContext is null)
        {
            return;
        }

        CultivateEntries.Filter = CultivateEntryViewFilter.Compile(SearchData, metadataContext);
        CultivateEntries.Refresh();
    }
}