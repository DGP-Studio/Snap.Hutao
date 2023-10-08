// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Control.Media;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Response;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using CalculatorAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalculatorClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalculatorConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using CalculatorItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;
using CalculatorItemHelper = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.ItemHelper;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 角色属性视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AvatarPropertyViewModel : Abstraction.ViewModel, IRecipient<UserChangedMessage>
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IAppResourceProvider appResourceProvider;
    private readonly ICultivationService cultivationService;
    private readonly IAvatarInfoService avatarInfoService;
    private readonly IClipboardInterop clipboardInterop;
    private readonly CalculatorClient calculatorClient;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    private Summary? summary;
    private AvatarView? selectedAvatar;

    /// <summary>
    /// 简述对象
    /// </summary>
    public Summary? Summary { get => summary; set => SetProperty(ref summary, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public AvatarView? SelectedAvatar { get => selectedAvatar; set => SetProperty(ref selectedAvatar, value); }

    /// <inheritdoc/>
    public void Receive(UserChangedMessage message)
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            RefreshCoreAsync(userAndUid, RefreshOption.None, CancellationToken).SafeForget();
        }
    }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.None, CancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    [Command("RefreshFromEnkaApiCommand")]
    private async Task RefreshByEnkaApiAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.RequestFromEnkaAPI, CancellationToken).ConfigureAwait(false);
        }
    }

    [Command("RefreshFromHoyolabGameRecordCommand")]
    private async Task RefreshByHoyolabGameRecordAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.RequestFromHoyolabGameRecord, CancellationToken).ConfigureAwait(false);
        }
    }

    [Command("RefreshFromHoyolabCalculateCommand")]
    private async Task RefreshByHoyolabCalculateAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            await RefreshCoreAsync(userAndUid, RefreshOption.RequestFromHoyolabCalculate, CancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask RefreshCoreAsync(UserAndUid userAndUid, RefreshOption option, CancellationToken token)
    {
        try
        {
            ValueResult<RefreshResult, Summary?> summaryResult;
            using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
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

            (RefreshResult result, Summary? summary) = summaryResult;
            if (result == RefreshResult.Ok)
            {
                await taskContext.SwitchToMainThreadAsync();
                Summary = summary;
                SelectedAvatar = Summary?.Avatars.FirstOrDefault();
            }
            else
            {
                switch (result)
                {
                    case RefreshResult.APIUnavailable:
                        infoBarService.Warning(SH.ViewModelAvatarPropertyEnkaApiUnavailable);
                        break;

                    case RefreshResult.StatusCodeNotSucceed:
                        ArgumentNullException.ThrowIfNull(summary);
                        infoBarService.Warning(summary.Message);
                        break;

                    case RefreshResult.ShowcaseNotOpen:
                        infoBarService.Warning(SH.ViewModelAvatarPropertyShowcaseNotOpen);
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

        if (userService.Current is null)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
        }
        else
        {
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

            Response<CalculatorConsumption> consumptionResponse = await calculatorClient
                .ComputeAsync(userService.Current.Entity, delta)
                .ConfigureAwait(false);

            if (!consumptionResponse.IsOk())
            {
                return;
            }

            CalculatorConsumption consumption = consumptionResponse.Data;

            List<CalculatorItem> items = CalculatorItemHelper.Merge(consumption.AvatarConsume, consumption.AvatarSkillConsume);
            bool avatarSaved = await cultivationService
                .SaveConsumptionAsync(CultivateType.AvatarAndSkill, avatar.Id, items)
                .ConfigureAwait(false);

            try
            {
                // take a hot path if avatar is not saved.
                bool avatarAndWeaponSaved = avatarSaved && await cultivationService
                    .SaveConsumptionAsync(CultivateType.Weapon, avatar.Weapon.Id, consumption.WeaponConsume.EmptyIfNull())
                    .ConfigureAwait(false);

                if (avatarAndWeaponSaved)
                {
                    infoBarService.Success(SH.ViewModelCultivationEntryAddSuccess);
                }
                else
                {
                    infoBarService.Warning(SH.ViewModelCultivationEntryAddWarning);
                }
            }
            catch (Core.ExceptionService.UserdataCorruptedException ex)
            {
                infoBarService.Error(ex, SH.ViewModelCultivationAddWarning);
            }
        }
    }

    [Command("BatchCultivateCommand")]
    private async Task BatchCultivateAsync()
    {
        if (summary is { Avatars: { } avatars })
        {
            if (userService.Current is null)
            {
                infoBarService.Warning(SH.MustSelectUserAndUid);
            }
            else
            {
                CultivatePromotionDeltaBatchDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivatePromotionDeltaBatchDialog>().ConfigureAwait(false);
                (bool isOk, CalculatorAvatarPromotionDelta baseline) = await dialog.GetPromotionDeltaBaselineAsync().ConfigureAwait(false);

                if (isOk)
                {
                    ArgumentNullException.ThrowIfNull(baseline.SkillList);
                    ArgumentNullException.ThrowIfNull(baseline.Weapon);

                    ContentDialog progressDialog = await contentDialogFactory
                        .CreateForIndeterminateProgressAsync(SH.ViewModelAvatarPropertyBatchCultivateProgressTitle)
                        .ConfigureAwait(false);
                    using (await progressDialog.BlockAsync(taskContext).ConfigureAwait(false))
                    {
                        BatchCultivateResult result = default;
                        foreach (AvatarView avatar in avatars)
                        {
                            baseline.AvatarId = avatar.Id;
                            baseline.AvatarLevelCurrent = Math.Min(avatar.LevelNumber, baseline.AvatarLevelTarget);

                            if (avatar.Skills.Count < 3)
                            {
                                continue;
                            }

                            baseline.SkillList[0].Id = avatar.Skills[0].GroupId;
                            baseline.SkillList[0].LevelCurrent = Math.Min(avatar.Skills[0].LevelNumber, baseline.SkillList[0].LevelTarget);
                            baseline.SkillList[1].Id = avatar.Skills[1].GroupId;
                            baseline.SkillList[1].LevelCurrent = Math.Min(avatar.Skills[1].LevelNumber, baseline.SkillList[1].LevelTarget);
                            baseline.SkillList[2].Id = avatar.Skills[2].GroupId;
                            baseline.SkillList[2].LevelCurrent = Math.Min(avatar.Skills[2].LevelNumber, baseline.SkillList[2].LevelTarget);

                            if (avatar.Weapon is null)
                            {
                                result.SkippedCount++;
                                continue;
                            }

                            baseline.Weapon.Id = avatar.Weapon.Id;
                            baseline.Weapon.LevelCurrent = Math.Min(avatar.Weapon.LevelNumber, baseline.Weapon.LevelTarget);

                            Response<CalculatorConsumption> consumptionResponse = await calculatorClient
                                .ComputeAsync(userService.Current.Entity, baseline)
                                .ConfigureAwait(false);

                            if (!consumptionResponse.IsOk())
                            {
                                result.Interrupted = true;
                                break;
                            }
                            else
                            {
                                CalculatorConsumption consumption = consumptionResponse.Data;

                                List<CalculatorItem> items = CalculatorItemHelper.Merge(consumption.AvatarConsume, consumption.AvatarSkillConsume);
                                bool avatarSaved = await cultivationService
                                    .SaveConsumptionAsync(CultivateType.AvatarAndSkill, avatar.Id, items)
                                    .ConfigureAwait(false);

                                try
                                {
                                    // take a hot path if avatar is not saved.
                                    bool avatarAndWeaponSaved = avatarSaved && await cultivationService
                                        .SaveConsumptionAsync(CultivateType.Weapon, avatar.Weapon.Id, consumption.WeaponConsume.EmptyIfNull())
                                        .ConfigureAwait(false);

                                    if (avatarAndWeaponSaved)
                                    {
                                        result.SucceedCount++;
                                    }
                                    else
                                    {
                                        result.Interrupted = true;
                                        break;
                                    }
                                }
                                catch (Core.ExceptionService.UserdataCorruptedException ex)
                                {
                                    infoBarService.Error(ex, SH.ViewModelCultivationAddWarning);
                                }
                            }
                        }

                        if (result.Interrupted)
                        {
                            infoBarService.Warning(SH.ViewModelCultivationEntryAddWarning);
                            infoBarService.Warning(SH.ViewModelCultivationBatchAddIncompletedFormat.Format(result.SucceedCount, result.SkippedCount));
                        }
                        else
                        {
                            infoBarService.Success(SH.ViewModelCultivationBatchAddCompletedFormat.Format(result.SucceedCount, result.SkippedCount));
                        }
                    }
                }
            }
        }
    }

    [Command("ExportAsImageCommand")]
    private async Task ExportAsImageAsync(FrameworkElement? element)
    {
        if (element is { IsLoaded: true })
        {
            RenderTargetBitmap bitmap = new();
            await bitmap.RenderAsync(element);

            IBuffer buffer = await bitmap.GetPixelsAsync();
            bool clipboardOpened = false;
            using (SoftwareBitmap softwareBitmap = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, bitmap.PixelWidth, bitmap.PixelHeight))
            {
                Bgra32 tint = appResourceProvider.GetResource<Color>("CompatBackgroundColor");
                softwareBitmap.NormalBlend(tint);
                using (InMemoryRandomAccessStream memory = new())
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memory);
                    encoder.SetSoftwareBitmap(softwareBitmap);
                    await encoder.FlushAsync();

                    clipboardOpened = clipboardInterop.SetBitmap(memory);
                }
            }

            if (clipboardOpened)
            {
                infoBarService.Success(SH.ViewModelAvatarPropertyExportImageSuccess);
            }
            else
            {
                infoBarService.Warning(SH.ViewModelAvatarPropertyOpenClipboardFail);
            }
        }
    }
}