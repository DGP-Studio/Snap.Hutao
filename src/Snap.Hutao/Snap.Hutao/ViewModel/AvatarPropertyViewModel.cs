// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Control;
using Snap.Hutao.Control.Image;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.Win32;
using Windows.Win32.System.WinRT;
using WinRT;

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

    private static unsafe void NormalBlend(SoftwareBitmap softwareBitmap, Bgra8 tint)
    {
        using (BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
        {
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                reference.As<IMemoryBufferByteAccess>().GetBuffer(out byte* data, out uint length);

                for (int i = 0; i < length; i += 4)
                {
                    Bgra8* pixel = (Bgra8*)(data + i);
                    byte baseAlpha = pixel->A;
                    pixel->B = (byte)(((pixel->B * baseAlpha) + (tint.B * (0xFF - baseAlpha))) / 0xFF);
                    pixel->G = (byte)(((pixel->G * baseAlpha) + (tint.G * (0xFF - baseAlpha))) / 0xFF);
                    pixel->R = (byte)(((pixel->R * baseAlpha) + (tint.R * (0xFF - baseAlpha))) / 0xFF);
                    pixel->A = 0xFF;
                }
            }
        }
    }

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
        NormalBlend(softwareBitmap, tint);

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