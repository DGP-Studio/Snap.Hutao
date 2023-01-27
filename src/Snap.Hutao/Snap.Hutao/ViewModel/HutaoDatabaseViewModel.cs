// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Hutao.Model;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 胡桃数据库视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class HutaoDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IHutaoCache hutaoCache;

    private List<ComplexAvatarRank>? avatarUsageRanks;
    private List<ComplexAvatarRank>? avatarAppearanceRanks;
    private List<ComplexAvatarConstellationInfo>? avatarConstellationInfos;
    private List<ComplexTeamRank>? teamAppearances;
    private Overview? overview;

    /// <summary>
    /// 构造一个新的胡桃数据库视图模型
    /// </summary>
    /// <param name="hutaoCache">胡桃服务缓存</param>
    public HutaoDatabaseViewModel(IHutaoCache hutaoCache)
    {
        this.hutaoCache = hutaoCache;

        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
        ExportAsImageCommand = new AsyncRelayCommand<UIElement>(ExportAsImageAsync);
    }

    /// <summary>
    /// 角色使用率
    /// </summary>
    public List<ComplexAvatarRank>? AvatarUsageRanks { get => avatarUsageRanks; set => SetProperty(ref avatarUsageRanks, value); }

    /// <summary>
    /// 角色上场率
    /// </summary>
    public List<ComplexAvatarRank>? AvatarAppearanceRanks { get => avatarAppearanceRanks; set => SetProperty(ref avatarAppearanceRanks, value); }

    /// <summary>
    /// 角色命座信息
    /// </summary>
    public List<ComplexAvatarConstellationInfo>? AvatarConstellationInfos { get => avatarConstellationInfos; set => SetProperty(ref avatarConstellationInfos, value); }

    /// <summary>
    /// 队伍出场
    /// </summary>
    public List<ComplexTeamRank>? TeamAppearances { get => teamAppearances; set => SetProperty(ref teamAppearances, value); }

    /// <summary>
    /// 总览数据
    /// </summary>
    public Overview? Overview { get => overview; set => SetProperty(ref overview, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 导出为图片命令
    /// </summary>
    public ICommand ExportAsImageCommand { get; }

    private async Task OpenUIAsync()
    {
        if (await hutaoCache.InitializeForDatabaseViewModelAsync().ConfigureAwait(true))
        {
            AvatarAppearanceRanks = hutaoCache.AvatarAppearanceRanks;
            AvatarUsageRanks = hutaoCache.AvatarUsageRanks;
            AvatarConstellationInfos = hutaoCache.AvatarConstellationInfos;
            TeamAppearances = hutaoCache.TeamAppearances;
            Overview = hutaoCache.Overview;
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
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        using (FileStream file = File.Create(Path.Combine(desktop, "hutao-database.png")))
        {
            using (IRandomAccessStream randomFileStream = file.AsRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, randomFileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96, 96, buffer.ToArray());
                await encoder.FlushAsync();
            }
        }
    }
}
