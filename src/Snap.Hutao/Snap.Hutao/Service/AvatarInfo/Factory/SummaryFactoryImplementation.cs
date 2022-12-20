// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Metadata;
using ModelAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using ModelPlayerInfo = Snap.Hutao.Web.Enka.Model.PlayerInfo;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 真正实现
/// </summary>
internal class SummaryFactoryImplementation
{
    private readonly SummaryMetadataContext metadataContext;

    /// <summary>
    /// 装配一个工厂实现
    /// </summary>
    /// <param name="metadataContext">元数据上下文</param>
    public SummaryFactoryImplementation(SummaryMetadataContext metadataContext)
    {
        this.metadataContext = metadataContext;
    }

    /// <summary>
    /// 创建一个新的属性统计对象
    /// </summary>
    /// <param name="playerInfo">玩家信息</param>
    /// <param name="avatarInfos">角色信息</param>
    /// <returns>属性统计</returns>
    public Summary Create(ModelPlayerInfo playerInfo, IEnumerable<ModelAvatarInfo> avatarInfos)
    {
        return new()
        {
            Player = SummaryHelper.CreatePlayer(playerInfo),
            Avatars = avatarInfos.Where(a => !AvatarIds.IsPlayer(a.AvatarId)).Select(a =>
            {
                SummaryAvatarFactory summaryAvatarFactory = new(metadataContext, a);
                return summaryAvatarFactory.CreateAvatar();
            }).ToList(),
        };
    }
}
