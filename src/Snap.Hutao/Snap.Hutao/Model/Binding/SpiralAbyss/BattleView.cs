// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.Model.Binding.SpiralAbyss;

/// <summary>
/// 上下半视图
/// </summary>
[HighQuality]
internal sealed class BattleView
{
    /// <summary>
    /// 构造一个新的上下半视图
    /// </summary>
    /// <param name="battle">战斗</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    public BattleView(Battle battle, Dictionary<AvatarId, Metadata.Avatar.Avatar> idAvatarMap)
    {
        Time = DateTimeOffset.FromUnixTimeSeconds(battle.Timestamp).ToLocalTime().ToString("yyyy.MM.dd HH:mm:ss");
        Avatars = battle.Avatars.Select(a => new AvatarView(a.Id, idAvatarMap)).ToList();
    }

    /// <summary>
    /// 时间
    /// </summary>
    public string Time { get; }

    /// <summary>
    /// 角色
    /// </summary>
    public List<AvatarView> Avatars { get; }
}