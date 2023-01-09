// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Control;
using Snap.Hutao.Control.Media;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Binding.Cultivation;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using CalcAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalcClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalcConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using CalcItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;
using CalcItemHelper = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.ItemHelper;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 角色属性视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class AvatarPropertyViewModel : ObservableObject, ISupportCancellation
{
    private readonly IUserService userService;
    private readonly IAvatarInfoService avatarInfoService;
    private readonly IInfoBarService infoBarService;
    private Summary? summary;
    private Avatar? selectedAvatar;

    /// <summary>
    /// 构造一个新的角色属性视图模型
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="avatarInfoService">角色信息服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="infoBarService">信息条服务</param>
    public AvatarPropertyViewModel(
        IUserService userService,
        IAvatarInfoService avatarInfoService,
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        IInfoBarService infoBarService)
    {
        this.userService = userService;
        this.avatarInfoService = avatarInfoService;
        this.infoBarService = infoBarService;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        RefreshFromEnkaApiCommand = asyncRelayCommandFactory.Create(RefreshByEnkaApiAsync);
        RefreshFromHoyolabGameRecordCommand = asyncRelayCommandFactory.Create(RefreshByHoyolabGameRecordAsync);
        RefreshFromHoyolabCalculateCommand = asyncRelayCommandFactory.Create(RefreshByHoyolabCalculateAsync);
        ExportAsImageCommand = asyncRelayCommandFactory.Create<UIElement>(ExportAsImageAsync);
        CultivateCommand = asyncRelayCommandFactory.Create<Avatar>(CultivateAsync);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 简述对象
    /// </summary>
    public Summary? Summary { get => summary; set => SetProperty(ref summary, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public Avatar? SelectedAvatar { get => selectedAvatar; set => SetProperty(ref selectedAvatar, value); }

    /// <summary>
    /// 加载界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

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

    private Task OpenUIAsync()
    {
        if (userService.Current is User user)
        {
            if (user.SelectedUserGameRole is UserGameRole role)
            {
                UserAndRole userAndRole = new(user.Entity, role);
                return RefreshCoreAsync(userAndRole, RefreshOption.None, CancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    private Task RefreshByEnkaApiAsync()
    {
        if (userService.Current is User user)
        {
            if (user.SelectedUserGameRole is UserGameRole role)
            {
                UserAndRole userAndRole = new(user.Entity, role);
                return RefreshCoreAsync(userAndRole, RefreshOption.RequestFromEnkaAPI, CancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    private Task RefreshByHoyolabGameRecordAsync()
    {
        if (userService.Current is User user)
        {
            if (user.SelectedUserGameRole is UserGameRole role)
            {
                UserAndRole userAndRole = new(user.Entity, role);
                return RefreshCoreAsync(userAndRole, RefreshOption.RequestFromHoyolabGameRecord, CancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    private Task RefreshByHoyolabCalculateAsync()
    {
        if (userService.Current is User user)
        {
            if (user.SelectedUserGameRole is UserGameRole role)
            {
                UserAndRole userAndRole = new(user.Entity, role);
                return RefreshCoreAsync(userAndRole, RefreshOption.RequestFromHoyolabCalculate, CancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    private async Task RefreshCoreAsync(UserAndRole userAndRole, RefreshOption option, CancellationToken token)
    {
        try
        {
            (RefreshResult result, Summary? summary) = await avatarInfoService.GetSummaryAsync(userAndRole, option, token).ConfigureAwait(false);

            if (result == RefreshResult.Ok)
            {
                await ThreadHelper.SwitchToMainThreadAsync();
                Summary = summary;
                SelectedAvatar = Summary?.Avatars.FirstOrDefault();
            }
            else
            {
                switch (result)
                {
                    case RefreshResult.APIUnavailable:
                        infoBarService.Warning("角色信息服务 [Enak API] 当前不可用");
                        break;
                    case RefreshResult.ShowcaseNotOpen:
                        infoBarService.Warning("角色橱窗尚未开启，请前往游戏操作后重试");
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task CultivateAsync(Avatar? avatar)
    {
        if (avatar != null)
        {
            IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
            IUserService userService = Ioc.Default.GetRequiredService<IUserService>();

            if (userService.Current != null)
            {
                (bool isOk, CalcAvatarPromotionDelta delta) = await new CultivatePromotionDeltaDialog(avatar.ToCalculable(), avatar.Weapon.ToCalculable())
                    .GetPromotionDeltaAsync()
                    .ConfigureAwait(false);

                if (isOk)
                {
                    CalcConsumption? consumption = await Ioc.Default
                        .GetRequiredService<CalcClient>()
                        .ComputeAsync(userService.Current.Entity, delta)
                        .ConfigureAwait(false);

                    if (consumption != null)
                    {
                        List<CalcItem> items = CalcItemHelper.Merge(consumption.AvatarConsume, consumption.AvatarSkillConsume);
                        bool avatarSaved = await Ioc.Default
                            .GetRequiredService<ICultivationService>()
                            .SaveConsumptionAsync(CultivateType.AvatarAndSkill, avatar.Id, items)
                            .ConfigureAwait(false);

                        bool weaponSaved = await Ioc.Default
                            .GetRequiredService<ICultivationService>()
                            .SaveConsumptionAsync(CultivateType.Weapon, avatar.Weapon.Id, consumption.WeaponConsume.EmptyIfNull())
                            .ConfigureAwait(false);

                        if (avatarSaved && weaponSaved)
                        {
                            infoBarService.Success("已成功添加至当前养成计划");
                        }
                        else
                        {
                            infoBarService.Warning("请先前往养成计划页面创建计划并选中");
                        }
                    }
                }
            }
            else
            {
                infoBarService.Warning("必须先选择一个用户与角色");
            }
        }
    }

    private async Task ExportAsImageAsync(UIElement? element)
    {
        if (element == null)
        {
            return;
        }

        RenderTargetBitmap bitmap = new();
        await bitmap.RenderAsync(element);

        IBuffer buffer = await bitmap.GetPixelsAsync();
        SoftwareBitmap softwareBitmap = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, bitmap.PixelWidth, bitmap.PixelHeight, BitmapAlphaMode.Ignore);
        Color tintColor = (Color)Ioc.Default.GetRequiredService<App>().Resources["CompatBackgroundColor"];
        Bgra8 tint = Bgra8.FromColor(tintColor);
        softwareBitmap.NormalBlend(tint);

        using (InMemoryRandomAccessStream memory = new())
        {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memory);
            encoder.SetSoftwareBitmap(softwareBitmap);
            await encoder.FlushAsync();
            Clipboard.SetBitmapStream(memory);
        }

        infoBarService.Success("已导出到剪贴板");
    }
}