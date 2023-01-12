// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Hutao.Model;

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
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public HutaoDatabaseViewModel(IHutaoCache hutaoCache, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.hutaoCache = hutaoCache;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
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
    public List<ComplexAvatarConstellationInfo>? AvatarConstellationInfos { get => avatarConstellationInfos; set => avatarConstellationInfos = value; }

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
}
