// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using Snap.Hutao.Web.Response;
using System.Globalization;
using CalculatorAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalculatorBatchConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.BatchConsumption;
using CalculatorClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalculatorConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using CalculatorItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;
using CalculatorItemHelper = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.ItemHelper;

namespace Snap.Hutao.ViewModel.AvatarProperty;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AvatarPropertyViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IAppResourceProvider appResourceProvider;
    private readonly ICultivationService cultivationService;
    private readonly IAvatarInfoService avatarInfoService;
    private readonly IClipboardProvider clipboardProvider;
    private readonly CalculatorClient calculatorClient;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    private Summary? summary;

    private enum CultivateCoreResult
    {
        Ok,
        ComputeConsumptionFailed,
        SaveConsumptionFailed,
    }

    public Summary? Summary { get => summary; set => SetProperty(ref summary, value); }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            RefreshCoreAsync(userAndUid, RefreshOption.None, CancellationToken).SafeForget();
        }
    }

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.None, CancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    [Command("RefreshFromEnkaApiCommand")]
    private async Task RefreshByEnkaApiAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.RequestFromEnkaAPI, CancellationToken).ConfigureAwait(false);
        }
    }

    [Command("RefreshFromHoyolabGameRecordCommand")]
    private async Task RefreshByHoyolabGameRecordAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.RequestFromHoyolabGameRecord, CancellationToken).ConfigureAwait(false);
        }
    }

    [Command("RefreshFromHoyolabCalculateCommand")]
    private async Task RefreshByHoyolabCalculateAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.RequestFromHoyolabCalculate, CancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask RefreshCoreAsync(UserAndUid userAndUid, RefreshOption option, CancellationToken token)
    {
        try
        {
            await taskContext.SwitchToMainThreadAsync();
            IsInitialized = false;
            ValueResult<RefreshResultKind, Summary?> summaryResult;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                ContentDialog dialog = await contentDialogFactory
                    .CreateForIndeterminateProgressAsync(SH.ViewModelAvatarPropertyFetch)
                    .ConfigureAwait(false);

                using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
                {
                    summaryResult = await avatarInfoService
                        .GetSummaryAsync(userAndUid, option, token)
                        .ConfigureAwait(false);
                }
            }

            (RefreshResultKind result, Summary? summary) = summaryResult;
            if (result is RefreshResultKind.Ok)
            {
                await taskContext.SwitchToMainThreadAsync();
                Summary = summary;
                Summary?.Avatars.MoveCurrentToFirstOrDefault();
            }
            else
            {
                switch (result)
                {
                    case RefreshResultKind.APIUnavailable:
                        infoBarService.Warning(SH.ViewModelAvatarPropertyEnkaApiUnavailable);
                        break;

                    case RefreshResultKind.StatusCodeNotSucceed:
                        ArgumentNullException.ThrowIfNull(summary);
                        infoBarService.Warning(summary.Message);
                        break;

                    case RefreshResultKind.ShowcaseNotOpen:
                        infoBarService.Warning(SH.ViewModelAvatarPropertyShowcaseNotOpen);
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            await taskContext.SwitchToMainThreadAsync();
            IsInitialized = true;
        }
    }

    [Command("CultivateCommand")]
    private async Task CultivateAsync(AvatarView? avatar)
    {
        if (avatar is null)
        {
            return;
        }

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        if (avatar.Weapon is null)
        {
            infoBarService.Warning(SH.ViewModelAvatarPropertyCalculateWeaponNull);
            return;
        }

        CalculableOptions options = new(avatar.ToCalculable(), avatar.Weapon.ToCalculable());
        CultivatePromotionDeltaDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivatePromotionDeltaDialog>(options).ConfigureAwait(false);
        (bool isOk, CalculatorAvatarPromotionDelta delta) = await dialog.GetPromotionDeltaAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        Response<CalculatorBatchConsumption> response = await calculatorClient.BatchComputeAsync(userAndUid, delta).ConfigureAwait(false);

        if (!response.IsOk())
        {
            return;
        }

        if (!await SaveCultivationAsync(response.Data.Items.Single(), delta).ConfigureAwait(false))
        {
            infoBarService.Warning(SH.ViewModelCultivationEntryAddWarning);
            return;
        }

        infoBarService.Success(SH.ViewModelCultivationEntryAddSuccess);
    }

    [Command("BatchCultivateCommand")]
    private async Task BatchCultivateAsync()
    {
        if (summary is not { Avatars: { } avatars })
        {
            return;
        }

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        CultivatePromotionDeltaBatchDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivatePromotionDeltaBatchDialog>().ConfigureAwait(false);
        (bool isOk, CalculatorAvatarPromotionDelta baseline) = await dialog.GetPromotionDeltaBaselineAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(baseline.SkillList);
        ArgumentNullException.ThrowIfNull(baseline.Weapon);

        ContentDialog progressDialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelAvatarPropertyBatchCultivateProgressTitle)
            .ConfigureAwait(false);

        BatchCultivateResult result = default;
        using (await progressDialog.BlockAsync(taskContext).ConfigureAwait(false))
        {
            List<CalculatorAvatarPromotionDelta> deltas = [];
            foreach (AvatarView avatar in avatars)
            {
                if (!baseline.TryGetNonErrorCopy(avatar, out CalculatorAvatarPromotionDelta? copy))
                {
                    ++result.SkippedCount;
                    continue;
                }

                deltas.Add(copy);
            }

            Response<CalculatorBatchConsumption> response = await calculatorClient.BatchComputeAsync(userAndUid, deltas).ConfigureAwait(false);

            if (!response.IsOk())
            {
                return;
            }

            foreach ((CalculatorConsumption consumption, CalculatorAvatarPromotionDelta delta) in response.Data.Items.Zip(deltas))
            {
                if (!await SaveCultivationAsync(consumption, delta).ConfigureAwait(false))
                {
                    result.Interrupted = true;
                    break;
                }

                ++result.SucceedCount;
            }
        }

        if (result.Interrupted)
        {
            infoBarService.Warning(SH.FormatViewModelCultivationBatchAddIncompletedFormat(result.SucceedCount, result.SkippedCount));
        }
        else
        {
            infoBarService.Success(SH.FormatViewModelCultivationBatchAddCompletedFormat(result.SucceedCount, result.SkippedCount));
        }
    }

    private async ValueTask<bool> SaveCultivationAsync(CalculatorConsumption consumption, CalculatorAvatarPromotionDelta delta)
    {
        LevelInformation levelInformation = LevelInformation.From(delta);

        List<CalculatorItem> items = CalculatorItemHelper.Merge(consumption.AvatarConsume, consumption.AvatarSkillConsume);
        bool avatarSaved = await cultivationService
            .SaveConsumptionAsync(CultivateType.AvatarAndSkill, delta.AvatarId, items, levelInformation)
            .ConfigureAwait(false);

        try
        {
            ArgumentNullException.ThrowIfNull(delta.Weapon);

            // Take a hot path if avatar is not saved.
            bool avatarAndWeaponSaved = avatarSaved && await cultivationService
                .SaveConsumptionAsync(CultivateType.Weapon, delta.Weapon.Id, consumption.WeaponConsume.EmptyIfNull(), levelInformation)
                .ConfigureAwait(false);

            return avatarAndWeaponSaved;
        }
        catch (HutaoException ex)
        {
            infoBarService.Error(ex, SH.ViewModelCultivationAddWarning);
        }

        return true;
    }

    [Command("ExportToTextCommand")]
    private void ExportToText()
    {
        if (Summary is not { Avatars.CurrentItem: { } avatar })
        {
            return;
        }

        if (clipboardProvider.SetText(AvatarViewTextTemplating.GetTemplatedText(avatar)))
        {
            infoBarService.Success(SH.ViewModelAvatatPropertyExportTextSuccess);
        }
        else
        {
            infoBarService.Warning(SH.ViewModelAvatatPropertyExportTextError);
        }
    }
}
