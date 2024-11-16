// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using Snap.Hutao.Web.Response;
using System.Collections.Frozen;
using System.Collections.ObjectModel;
using CalculatorAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalculatorBatchConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.BatchConsumption;
using CalculatorConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using CalculatorItemHelper = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.ItemHelper;

namespace Snap.Hutao.ViewModel.AvatarProperty;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AvatarPropertyViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>
{
    private readonly AvatarPropertyViewModelScopeContext scopeContext;

    private Summary? summary;
    private FrozenDictionary<string, SearchToken> availableTokens;

    public Summary? Summary { get => summary; set => SetProperty(ref summary, value); }

    public ObservableCollection<SearchToken>? FilterTokens { get; set => SetProperty(ref field, value); }

    public string? FilterToken { get; set => SetProperty(ref field, value); }

    public FrozenDictionary<string, SearchToken> AvailableTokens { get => availableTokens; set => SetProperty(ref availableTokens, value); }

    public string TotalAvatarCount
    {
        get => SH.FormatViewModelAvatarPropertyTotalAvatarCountHint(summary?.Avatars.Count ?? 0);
    }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            _ = RefreshCoreAsync(userAndUid, RefreshOption.None, CancellationToken);
        }
    }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        FilterTokens = [];
        availableTokens = FrozenDictionary.ToFrozenDictionary(
        [
            .. IntrinsicFrozen.ElementNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ElementName, nv.Name, nv.Value, iconUri: ElementNameIconConverter.ElementNameToUri(nv.Name)))),
            .. IntrinsicFrozen.ItemQualityNameValues.Where(nv => nv.Value >= QualityType.QUALITY_PURPLE).Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ItemQuality, nv.Name, (int)nv.Value, quality: QualityColorConverter.QualityToColor(nv.Value)))),
            .. IntrinsicFrozen.WeaponTypeNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.WeaponType, nv.Name, (int)nv.Value, iconUri: WeaponTypeIconConverter.WeaponTypeToIconUri(nv.Value)))),
        ]);

        if (await scopeContext.UserService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.None, CancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    [Command("RefreshFromHoyolabGameRecordCommand")]
    private async Task RefreshByHoyolabGameRecordAsync()
    {
        if (await scopeContext.UserService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.RequestFromHoyolabGameRecord, CancellationToken).ConfigureAwait(false);
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task RefreshCoreAsync(UserAndUid userAndUid, RefreshOption option, CancellationToken token)
    {
        try
        {
            await scopeContext.TaskContext.SwitchToMainThreadAsync();
            Summary = default;

            ValueResult<RefreshResultKind, Summary?> summaryResult;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                ContentDialog dialog = await scopeContext.ContentDialogFactory
                    .CreateForIndeterminateProgressAsync(SH.ViewModelAvatarPropertyFetch)
                    .ConfigureAwait(false);

                using (await scopeContext.ContentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
                {
                    summaryResult = await scopeContext.AvatarInfoService
                        .GetSummaryAsync(userAndUid, option, token)
                        .ConfigureAwait(false);
                }
            }

            (RefreshResultKind result, Summary? summary) = summaryResult;
            if (result is RefreshResultKind.Ok)
            {
                await scopeContext.TaskContext.SwitchToMainThreadAsync();
                Summary = summary;
                Summary?.Avatars.MoveCurrentToFirstOrDefault();
            }
            else
            {
                switch (result)
                {
                    case RefreshResultKind.APIUnavailable:
                        scopeContext.InfoBarService.Warning(SH.ViewModelAvatarPropertyEnkaApiUnavailable);
                        break;

                    case RefreshResultKind.StatusCodeNotSucceed:
                        ArgumentNullException.ThrowIfNull(summary);
                        scopeContext.InfoBarService.Warning(summary.Message);
                        break;

                    case RefreshResultKind.ShowcaseNotOpen:
                        scopeContext.InfoBarService.Warning(SH.ViewModelAvatarPropertyShowcaseNotOpen);
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Command("CultivateCommand")]
    private async Task CultivateAsync(AvatarView? avatar)
    {
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

        CalculableOptions options = new(avatar.ToCalculable(), avatar.Weapon.ToCalculable());
        CultivatePromotionDeltaDialog dialog = await scopeContext.ContentDialogFactory
            .CreateInstanceAsync<CultivatePromotionDeltaDialog>(options).ConfigureAwait(false);
        (bool isOk, CultivatePromotionDeltaOptions deltaOptions) = await dialog.GetPromotionDeltaAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        CalculatorBatchConsumption? batchConsumption;
        using (IServiceScope scope = scopeContext.ServiceScopeFactory.CreateScope())
        {
            CalculateClient calculatorClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();
            Response<CalculatorBatchConsumption> response = await calculatorClient
                .BatchComputeAsync(userAndUid, deltaOptions.Delta).ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(response, scopeContext.InfoBarService, out batchConsumption))
            {
                return;
            }
        }

        if (!await SaveCultivationAsync(batchConsumption.Items.Single(), deltaOptions).ConfigureAwait(false))
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelCultivationEntryAddWarning);
            return;
        }

        scopeContext.InfoBarService.Success(SH.ViewModelCultivationEntryAddSuccess);
    }

    [Command("BatchCultivateCommand")]
    private async Task BatchCultivateAsync()
    {
        if (summary is not { Avatars: { } avatars })
        {
            return;
        }

        if (await scopeContext.UserService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            scopeContext.InfoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        CultivatePromotionDeltaBatchDialog dialog = await scopeContext.ContentDialogFactory
            .CreateInstanceAsync<CultivatePromotionDeltaBatchDialog>().ConfigureAwait(false);
        (bool isOk, CultivatePromotionDeltaOptions deltaOptions) = await dialog.GetPromotionDeltaBaselineAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(deltaOptions.Delta.Weapon);

        ContentDialog progressDialog = await scopeContext.ContentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelAvatarPropertyBatchCultivateProgressTitle)
            .ConfigureAwait(false);

        BatchCultivateResult result = default;
        using (await scopeContext.ContentDialogFactory.BlockAsync(progressDialog).ConfigureAwait(false))
        {
            List<CalculatorAvatarPromotionDelta> deltas = [];
            foreach (AvatarView avatar in avatars)
            {
                if (!deltaOptions.Delta.TryGetNonErrorCopy(avatar, out CalculatorAvatarPromotionDelta? copy))
                {
                    ++result.SkippedCount;
                    continue;
                }

                deltas.Add(copy);
            }

            CalculatorBatchConsumption? batchConsumption;
            using (IServiceScope scope = scopeContext.ServiceScopeFactory.CreateScope())
            {
                CalculateClient calculatorClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();
                Response<CalculatorBatchConsumption> response = await calculatorClient.BatchComputeAsync(userAndUid, deltas).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scopeContext.InfoBarService, out batchConsumption))
                {
                    return;
                }
            }

            foreach ((CalculatorConsumption consumption, CalculatorAvatarPromotionDelta delta) in batchConsumption.Items.Zip(deltas))
            {
                if (!await SaveCultivationAsync(consumption, new(delta, deltaOptions.Strategy)).ConfigureAwait(false))
                {
                    break;
                }

                ++result.SucceedCount;
            }
        }

        if (result.SkippedCount > 0)
        {
            scopeContext.InfoBarService.Warning(SH.FormatViewModelCultivationBatchAddIncompletedFormat(result.SucceedCount, result.SkippedCount));
        }
        else
        {
            scopeContext.InfoBarService.Success(SH.FormatViewModelCultivationBatchAddCompletedFormat(result.SucceedCount, result.SkippedCount));
        }
    }

    /// <returns><see langword="true"/> if we can continue saving consumptions, otherwise <see langword="false"/>.</returns>
    private async ValueTask<bool> SaveCultivationAsync(CalculatorConsumption consumption, CultivatePromotionDeltaOptions options)
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
            ConsumptionSaveResultKind.Skipped => scopeContext.InfoBarService.Information(SH.ViewModelCultivationConsumptionSaveSkippedHint),
            ConsumptionSaveResultKind.NoItem => scopeContext.InfoBarService.Information(SH.ViewModelCultivationConsumptionSaveNoItemHint),
            ConsumptionSaveResultKind.Added => scopeContext.InfoBarService.Success(SH.ViewModelCultivationEntryAddSuccess),
            _ => default,
        };

        if (avatarSaveKind is ConsumptionSaveResultKind.NoProject)
        {
            return false;
        }

        try
        {
            ArgumentNullException.ThrowIfNull(options.Delta.Weapon);

            InputConsumption weaponInput = new()
            {
                Type = CultivateType.Weapon,
                ItemId = options.Delta.Weapon.Id,
                Items = consumption.WeaponConsume.EmptyIfNull(),
                LevelInformation = levelInformation,
                Strategy = options.Strategy,
            };

            ConsumptionSaveResultKind weaponSaveKind = await scopeContext.CultivationService.SaveConsumptionAsync(weaponInput).ConfigureAwait(false);
            _ = weaponSaveKind switch
            {
                ConsumptionSaveResultKind.NoProject => scopeContext.InfoBarService.Warning(SH.ViewModelCultivationEntryAddWarning),
                ConsumptionSaveResultKind.Skipped => scopeContext.InfoBarService.Information(SH.ViewModelCultivationConsumptionSaveSkippedHint),
                ConsumptionSaveResultKind.NoItem => scopeContext.InfoBarService.Information(SH.ViewModelCultivationConsumptionSaveNoItemHint),
                ConsumptionSaveResultKind.Added => scopeContext.InfoBarService.Success(SH.ViewModelCultivationEntryAddSuccess),
                _ => default,
            };

            return weaponSaveKind is not ConsumptionSaveResultKind.NoProject;
        }
        catch (HutaoException ex)
        {
            scopeContext.InfoBarService.Error(ex, SH.ViewModelCultivationAddWarning);
        }

        return false;
    }

    [Command("ExportToTextCommand")]
    private void ExportToText()
    {
        if (Summary is not { Avatars.CurrentItem: { } avatar })
        {
            return;
        }

        if (scopeContext.ClipboardProvider.SetText(AvatarViewTextTemplating.GetTemplatedText(avatar)))
        {
            scopeContext.InfoBarService.Success(SH.ViewModelAvatatPropertyExportTextSuccess);
        }
        else
        {
            scopeContext.InfoBarService.Warning(SH.ViewModelAvatatPropertyExportTextError);
        }
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        if (Summary is null)
        {
            return;
        }

        Summary.Avatars.Filter = FilterTokens is null or [] ? default! : AvatarViewFilter.Compile(FilterTokens);
        OnPropertyChanged(nameof(TotalAvatarCount));
    }
}