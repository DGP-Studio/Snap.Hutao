// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Service.Cultivation.Offline;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using CalculatorAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalculatorBatchConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.BatchConsumption;
using CalculatorConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using CalculatorItemHelper = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.ItemHelper;

namespace Snap.Hutao.ViewModel.AvatarProperty;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AvatarPropertyViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>, IDisposable
{
    private readonly ExclusiveTokenProvider refreshTokenProvider = new();
    private readonly AvatarPropertyViewModelScopeContext scopeContext;

    private SummaryFactoryMetadataContext? metadataContext;

    public Summary? Summary { get; set => SetProperty(ref field, value); }

    public SearchData? SearchData { get; set => SetProperty(ref field, value); }

    public string FormattedTotalAvatarCount { get => SH.FormatViewModelAvatarPropertyTotalAvatarCountHint(Summary?.Avatars.Count ?? 0); }

    public ImmutableArray<NameValue<AvatarPropertySortDescriptionKind>> SortDescriptionKinds { get; } = ImmutableCollectionsNameValue.FromEnum<AvatarPropertySortDescriptionKind>(type => type.GetLocalizedDescription());

    public NameValue<AvatarPropertySortDescriptionKind>? SortDescriptionKind
    {
        get => field ??= Selection.Initialize(SortDescriptionKinds, UnsafeLocalSetting.Get(SettingKeys.AvatarPropertySortDescriptionKind, AvatarPropertySortDescriptionKind.Default));
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                UnsafeLocalSetting.Set(SettingKeys.AvatarPropertySortDescriptionKind, value.Value);
                if (Summary?.Avatars is not { } avatars)
                {
                    return;
                }

                using (avatars.DeferRefresh())
                {
                    avatars.SortDescriptions.Clear();
                    foreach (ref readonly SortDescription sd in AvatarPropertySortDescriptions.Get(value.Value).AsSpan())
                    {
                        avatars.SortDescriptions.Add(sd);
                    }
                }

                avatars.MoveCurrentToFirst();
            }
        }
    }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is not { } userAndUid)
        {
            return;
        }

        CancellationToken token = refreshTokenProvider.GetNewToken();
        WeakReference<AvatarPropertyViewModel> weakThis = new(this);

        // 1. We need to wait for the view initialization (mainly for metadata context).
        // 2. We need to refresh summary data. otherwise, the view can be un-synced.
        Initialization.Task.ContinueWith(
            init =>
            {
                if (!init.Result)
                {
                    return;
                }

                if (weakThis.TryGetTarget(out AvatarPropertyViewModel? viewModel) && !viewModel.IsViewUnloaded)
                {
                    using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, viewModel.CancellationToken);
                    viewModel.PrivateRefreshAsync(userAndUid, RefreshOptionKind.None, linkedCts.Token).SafeForget();
                }
            },
            token,
            TaskContinuationOptions.None,
            TaskScheduler.Current);
    }

    public override void Dispose()
    {
        refreshTokenProvider.Dispose();
        base.Dispose();
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await scopeContext.MetadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        metadataContext = await scopeContext.MetadataService.GetContextAsync<SummaryFactoryMetadataContext>(token).ConfigureAwait(false);
        SearchData searchData = SearchData.CreateForAvatarProperty();

        if (await scopeContext.UserService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, refreshTokenProvider.GetNewToken());
            await PrivateRefreshAsync(userAndUid, RefreshOptionKind.None, linkedCts.Token).ConfigureAwait(false);
        }

        await scopeContext.TaskContext.SwitchToMainThreadAsync();
        SearchData = searchData;

        return true;
    }

    [Command("RefreshFromHoyolabGameRecordCommand")]
    private async Task RefreshByHoyolabGameRecordAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh", "AvatarPropertyViewModel.Command"));

        if (await scopeContext.UserService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, refreshTokenProvider.GetNewToken());
            await PrivateRefreshAsync(userAndUid, RefreshOptionKind.RequestFromHoyolabGameRecord, linkedCts.Token).ConfigureAwait(false);
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task PrivateRefreshAsync(UserAndUid userAndUid, RefreshOptionKind optionKind, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(metadataContext);

        try
        {
            Summary? summary;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                ContentDialog dialog = await scopeContext.ContentDialogFactory
                    .CreateForIndeterminateProgressAsync(SH.ViewModelAvatarPropertyFetch)
                    .ConfigureAwait(false);
                token.ThrowIfCancellationRequested();

                using (await scopeContext.ContentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
                {
                    token.ThrowIfCancellationRequested();
                    summary = await scopeContext.AvatarInfoService
                        .GetSummaryAsync(metadataContext, userAndUid, optionKind, token)
                        .ConfigureAwait(false);
                }
            }

            await scopeContext.TaskContext.SwitchToMainThreadAsync();
            token.ThrowIfCancellationRequested();
            Summary = summary;
            Summary?.Avatars.MoveCurrentToFirst();
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Command("CultivateCommand")]
    private async Task CultivateAsync(AvatarView? avatar)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Cultivate", "AvatarPropertyViewModel.Command"));

        ArgumentNullException.ThrowIfNull(metadataContext);

        if (avatar is null)
        {
            return;
        }

        if (await scopeContext.UserService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            scopeContext.InfoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        if (avatar.Weapon is null)
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelAvatarPropertyCalculateWeaponNull);
            return;
        }

        CalculableOptions options = new(avatar, avatar.Weapon);
        CultivatePromotionDeltaDialog dialog = await scopeContext.ContentDialogFactory
            .CreateInstanceAsync<CultivatePromotionDeltaDialog>(scopeContext.ServiceProvider, options)
            .ConfigureAwait(false);

        if (await dialog.GetPromotionDeltaAsync().ConfigureAwait(false) is not (true, { } deltaOptions))
        {
            return;
        }

        CalculatorBatchConsumption? batchConsumption;
        if (LocalSetting.Get(SettingKeys.EnableOfflineCultivationCalculator, false))
        {
            batchConsumption = OfflineCalculator.CalculateBatchConsumption(deltaOptions.Delta, metadataContext);
            ArgumentNullException.ThrowIfNull(batchConsumption);
        }
        else
        {
            using (IServiceScope scope = scopeContext.ServiceScopeFactory.CreateScope())
            {
                CalculateClient calculatorClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();
                Response<CalculatorBatchConsumption> response = await calculatorClient
                    .BatchComputeAsync(userAndUid, deltaOptions.Delta)
                    .ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scopeContext.InfoBarService, out batchConsumption))
                {
                    return;
                }
            }
        }

        if (!await SaveCultivationAsync(batchConsumption.Items.Single(), deltaOptions).ConfigureAwait(false))
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelCultivationEntryAddWarning);
        }
    }

    [Command("BatchCultivateCommand")]
    private async Task BatchCultivateAsync(bool full)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI($"Batch cultivate, full: {full}", "AvatarPropertyViewModel.Command"));

        ArgumentNullException.ThrowIfNull(metadataContext);

        if (Summary is not { Avatars: { } avatars })
        {
            return;
        }

        if (await scopeContext.UserService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            scopeContext.InfoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        ImmutableArray<AvatarView> targetAvatars;

        if (!full)
        {
            AvatarPropertyMultiAvatarCultivateSelectDialog selectDialog = await scopeContext.ContentDialogFactory
                .CreateInstanceAsync<AvatarPropertyMultiAvatarCultivateSelectDialog>(scopeContext.ServiceProvider)
                .ConfigureAwait(false);

            await scopeContext.TaskContext.SwitchToMainThreadAsync();
            selectDialog.Avatars = avatars;
            if (!await selectDialog.SelectAvatarsAsync().ConfigureAwait(false))
            {
                return;
            }

            if (!selectDialog.SelectedAvatars.Any())
            {
                scopeContext.InfoBarService.Warning(SH.ViewModelAvatarPropertyBatchCultivateNoSelectedAvatar);
                return;
            }

            targetAvatars = selectDialog.SelectedAvatars;
        }
        else
        {
            targetAvatars = [.. avatars.Source];
        }

        CultivatePromotionDeltaBatchDialog dialog = await scopeContext.ContentDialogFactory
            .CreateInstanceAsync<CultivatePromotionDeltaBatchDialog>(scopeContext.ServiceProvider)
            .ConfigureAwait(false);

        if (await dialog.GetPromotionDeltaBaselineAsync().ConfigureAwait(false) is not (true, { } baseline))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(baseline.Delta.Weapon);

        ContentDialog progressDialog = await scopeContext.ContentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelAvatarPropertyBatchCultivateProgressTitle)
            .ConfigureAwait(false);

        BatchCultivateResult result = default;
        using (await scopeContext.ContentDialogFactory.BlockAsync(progressDialog).ConfigureAwait(false))
        {
            ImmutableArray<CalculatorAvatarPromotionDelta>.Builder deltasBuilder = ImmutableArray.CreateBuilder<CalculatorAvatarPromotionDelta>();
            foreach (AvatarView avatar in targetAvatars)
            {
                if (!baseline.Delta.TryGetNonErrorCopy(avatar, out CalculatorAvatarPromotionDelta? copy))
                {
                    ++result.SkippedCount;
                    continue;
                }

                deltasBuilder.Add(copy);
            }

            ImmutableArray<CalculatorAvatarPromotionDelta> deltas = deltasBuilder.ToImmutable();

            CalculatorBatchConsumption? batchConsumption;
            if (LocalSetting.Get(SettingKeys.EnableOfflineCultivationCalculator, false))
            {
                batchConsumption = OfflineCalculator.CalculateBatchConsumption(deltas, metadataContext);
                ArgumentNullException.ThrowIfNull(batchConsumption);
            }
            else
            {
                using (IServiceScope scope = scopeContext.ServiceScopeFactory.CreateScope())
                {
                    CalculateClient calculatorClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();
                    Response<CalculatorBatchConsumption> response = await calculatorClient.BatchComputeAsync(userAndUid, deltas).ConfigureAwait(false);

                    if (!ResponseValidator.TryValidate(response, scopeContext.InfoBarService, out batchConsumption))
                    {
                        return;
                    }
                }
            }

            foreach ((CalculatorConsumption consumption, CalculatorAvatarPromotionDelta delta) in batchConsumption.Items.Zip(deltas))
            {
                if (!await SaveCultivationAsync(consumption, new(delta, baseline.Strategy), true).ConfigureAwait(false))
                {
                    break;
                }

                ++result.SucceedCount;
            }
        }

        _ = result.SkippedCount > 0
            ? scopeContext.InfoBarService.Warning(SH.FormatViewModelCultivationBatchAddIncompletedFormat(result.SucceedCount, result.SkippedCount))
            : scopeContext.InfoBarService.Success(SH.FormatViewModelCultivationBatchAddCompletedFormat(result.SucceedCount, result.SkippedCount));
    }

    /// <returns><see langword="true"/> if we can continue saving consumptions, otherwise <see langword="false"/>.</returns>
    private async ValueTask<bool> SaveCultivationAsync(CalculatorConsumption consumption, CultivatePromotionDeltaOptions options, bool isBatch = false)
    {
        LevelInformation levelInformation = LevelInformation.From(options.Delta);

        InputConsumption avatarInput = new()
        {
            Type = CultivateType.AvatarAndSkill,
            ItemId = options.Delta.AvatarId,
            Items = CalculatorItemHelper.Merge(consumption.AvatarConsume, consumption.AvatarSkillConsume),
            LevelInformation = levelInformation,
            Strategy = options.Strategy,
        };

        ConsumptionSaveResultKind avatarSaveKind = await scopeContext.CultivationService.SaveConsumptionAsync(avatarInput).ConfigureAwait(false);

        _ = avatarSaveKind switch
        {
            ConsumptionSaveResultKind.NoProject => scopeContext.InfoBarService.Warning(SH.ViewModelCultivationEntryAddWarning),
            ConsumptionSaveResultKind.Skipped => isBatch ? default : scopeContext.InfoBarService.Information(SH.ViewModelCultivationConsumptionSaveSkippedHint),
            ConsumptionSaveResultKind.NoItem => isBatch ? default : scopeContext.InfoBarService.Information(SH.ViewModelCultivationConsumptionSaveNoItemHint),
            ConsumptionSaveResultKind.Added => scopeContext.InfoBarService.Success(SH.ViewModelCultivationEntryAddSuccess),
            _ => default,
        };

        if (avatarSaveKind is ConsumptionSaveResultKind.NoProject)
        {
            return false;
        }

        ArgumentNullException.ThrowIfNull(options.Delta.Weapon);

        InputConsumption weaponInput = new()
        {
            Type = CultivateType.Weapon,
            ItemId = options.Delta.Weapon.Id,
            Items = consumption.WeaponConsume,
            LevelInformation = levelInformation,
            Strategy = options.Strategy,
        };

        ConsumptionSaveResultKind weaponSaveKind = await scopeContext.CultivationService.SaveConsumptionAsync(weaponInput).ConfigureAwait(false);
        _ = weaponSaveKind switch
        {
            ConsumptionSaveResultKind.NoProject => scopeContext.InfoBarService.Warning(SH.ViewModelCultivationEntryAddWarning),
            ConsumptionSaveResultKind.Skipped => isBatch ? default : scopeContext.InfoBarService.Information(SH.ViewModelCultivationConsumptionSaveSkippedHint),
            ConsumptionSaveResultKind.NoItem => isBatch ? default : scopeContext.InfoBarService.Information(SH.ViewModelCultivationConsumptionSaveNoItemHint),
            ConsumptionSaveResultKind.Added => scopeContext.InfoBarService.Success(SH.ViewModelCultivationEntryAddSuccess),
            _ => default,
        };

        return weaponSaveKind is not ConsumptionSaveResultKind.NoProject;
    }

    [Command("ExportToTextCommand")]
    private async Task ExportToTextAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Export as text to ClipBoard", "AvatarPropertyViewModel.Command"));

        if (Summary is not { Avatars.CurrentItem: { } avatar })
        {
            return;
        }

        _ = await scopeContext.ClipboardProvider.SetTextAsync(AvatarViewTextTemplating.GetTemplatedText(avatar)).ConfigureAwait(false)
            ? scopeContext.InfoBarService.Success(SH.ViewModelAvatatPropertyExportTextSuccess)
            : scopeContext.InfoBarService.Warning(SH.ViewModelAvatatPropertyExportTextError);
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Filter", "AvatarPropertyViewModel.Command"));

        if (Summary is null)
        {
            return;
        }

        Summary.Avatars.Filter = AvatarViewFilter.Compile(SearchData);
        OnPropertyChanged(nameof(FormattedTotalAvatarCount));

        if (Summary.Avatars.CurrentItem is null)
        {
            Summary.Avatars.MoveCurrentToFirst();
        }
    }
}