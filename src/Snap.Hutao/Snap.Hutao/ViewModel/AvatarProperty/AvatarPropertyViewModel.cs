// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Control.Media;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Response;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using CalcAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalcClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalcConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using CalcItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;
using CalcItemHelper = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.ItemHelper;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 角色属性视图模型
/// TODO: support page unload as cancellation
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class AvatarPropertyViewModel : Abstraction.ViewModel, IRecipient<UserChangedMessage>
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IInfoBarService infoBarService;
    private Summary? summary;
    private AvatarView? selectedAvatar;

    /// <summary>
    /// 构造一个新的角色属性视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public AvatarPropertyViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        userService = serviceProvider.GetRequiredService<IUserService>();
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        this.serviceProvider = serviceProvider;

        RefreshFromEnkaApiCommand = new AsyncRelayCommand(RefreshByEnkaApiAsync);
        RefreshFromHoyolabGameRecordCommand = new AsyncRelayCommand(RefreshByHoyolabGameRecordAsync);
        RefreshFromHoyolabCalculateCommand = new AsyncRelayCommand(RefreshByHoyolabCalculateAsync);
        ExportAsImageCommand = new AsyncRelayCommand<FrameworkElement>(ExportAsImageAsync);
        CultivateCommand = new AsyncRelayCommand<AvatarView>(CultivateAsync);

        serviceProvider.GetRequiredService<IMessenger>().Register(this);
    }

    /// <summary>
    /// 简述对象
    /// </summary>
    public Summary? Summary { get => summary; set => SetProperty(ref summary, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public AvatarView? SelectedAvatar { get => selectedAvatar; set => SetProperty(ref selectedAvatar, value); }

    /// <summary>
    /// 从 Enka Api 同步命令
    /// </summary>
    public ICommand RefreshFromEnkaApiCommand { get; }

    /// <summary>
    /// 从游戏记录同步命令
    /// </summary>
    public ICommand RefreshFromHoyolabGameRecordCommand { get; set; }

    /// <summary>
    /// 从养成计算同步命令
    /// </summary>
    public ICommand RefreshFromHoyolabCalculateCommand { get; }

    /// <summary>
    /// 导出图片命令
    /// </summary>
    public ICommand ExportAsImageCommand { get; }

    /// <summary>
    /// 养成命令
    /// </summary>
    public ICommand CultivateCommand { get; }

    /// <inheritdoc/>
    public void Receive(UserChangedMessage message)
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            RefreshCoreAsync(userAndUid, RefreshOption.None, CancellationToken).SafeForget();
        }
    }

    /// <inheritdoc/>
    protected override Task OpenUIAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            return RefreshCoreAsync(userAndUid, RefreshOption.None, CancellationToken);
        }

        return Task.CompletedTask;
    }

    private Task RefreshByEnkaApiAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            return RefreshCoreAsync(userAndUid, RefreshOption.RequestFromEnkaAPI, CancellationToken);
        }

        return Task.CompletedTask;
    }

    private Task RefreshByHoyolabGameRecordAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            return RefreshCoreAsync(userAndUid, RefreshOption.RequestFromHoyolabGameRecord, CancellationToken);
        }

        return Task.CompletedTask;
    }

    private Task RefreshByHoyolabCalculateAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            return RefreshCoreAsync(userAndUid, RefreshOption.RequestFromHoyolabCalculate, CancellationToken);
        }

        return Task.CompletedTask;
    }

    private async Task RefreshCoreAsync(UserAndUid userAndUid, RefreshOption option, CancellationToken token)
    {
        try
        {
            ValueResult<RefreshResult, Summary?> summaryResult;
            using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
            {
                ContentDialog dialog = await serviceProvider
                    .GetRequiredService<IContentDialogFactory>()
                    .CreateForIndeterminateProgressAsync(SH.ViewModelAvatarPropertyFetch)
                    .ConfigureAwait(false);

                using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
                {
                    summaryResult = await serviceProvider
                        .GetRequiredService<IAvatarInfoService>()
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
                        infoBarService.Warning(summary!.Message);
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

    private async Task CultivateAsync(AvatarView? avatar)
    {
        if (avatar != null)
        {
            if (userService.Current != null)
            {
                if (avatar.Weapon == null)
                {
                    infoBarService.Warning(SH.ViewModelAvatarPropertyCalculateWeaponNull);
                    return;
                }

                // ContentDialog must be created by main thread.
                await taskContext.SwitchToMainThreadAsync();
                CultivatePromotionDeltaDialog dialog = serviceProvider.CreateInstance<CultivatePromotionDeltaDialog>(avatar.ToCalculable(), avatar.Weapon.ToCalculable());
                (bool isOk, CalcAvatarPromotionDelta delta) = await dialog.GetPromotionDeltaAsync().ConfigureAwait(false);

                if (isOk)
                {
                    Response<CalcConsumption> consumptionResponse = await serviceProvider
                        .GetRequiredService<CalcClient>()
                        .ComputeAsync(userService.Current.Entity, delta)
                        .ConfigureAwait(false);

                    if (consumptionResponse.IsOk())
                    {
                        ICultivationService cultivationService = serviceProvider.GetRequiredService<ICultivationService>();
                        CalcConsumption consumption = consumptionResponse.Data;

                        List<CalcItem> items = CalcItemHelper.Merge(consumption.AvatarConsume, consumption.AvatarSkillConsume);
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
            }
            else
            {
                infoBarService.Warning(SH.MustSelectUserAndUid);
            }
        }
    }

    private async Task ExportAsImageAsync(FrameworkElement? element)
    {
        if (element != null && element.IsLoaded)
        {
            RenderTargetBitmap bitmap = new();
            await bitmap.RenderAsync(element);

            IBuffer buffer = await bitmap.GetPixelsAsync();
            bool clipboardOpened = false;
            using (SoftwareBitmap softwareBitmap = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, bitmap.PixelWidth, bitmap.PixelHeight))
            {
                Color tintColor = serviceProvider.GetRequiredService<IAppResourceProvider>().GetResource<Color>("CompatBackgroundColor");
                Bgra8 tint = Bgra8.FromColor(tintColor);
                softwareBitmap.NormalBlend(tint);
                using (InMemoryRandomAccessStream memory = new())
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memory);
                    encoder.SetSoftwareBitmap(softwareBitmap);
                    await encoder.FlushAsync();

                    clipboardOpened = serviceProvider.GetRequiredService<IClipboardInterop>().SetBitmap(memory);
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