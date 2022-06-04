// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 玩家记录
/// 使用 <see cref="Create(string, List{Character}, SpiralAbyss)"/> 来构建一个实例
/// </summary>
public class PlayerRecord
{
    /// <summary>
    /// 构造一个新的玩家记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="playerAvatars">玩家角色</param>
    /// <param name="playerSpiralAbyssesLevels">玩家深渊信息</param>
    private PlayerRecord(string uid, IEnumerable<PlayerAvatar> playerAvatars, IEnumerable<PlayerSpiralAbyssLevel> playerSpiralAbyssesLevels)
    {
        Uid = uid;
        PlayerAvatars = playerAvatars;
        PlayerSpiralAbyssesLevels = playerSpiralAbyssesLevels;
    }

    /// <summary>
    /// uid
    /// </summary>
    public string Uid { get; }

    /// <summary>
    /// 玩家角色
    /// </summary>
    public IEnumerable<PlayerAvatar> PlayerAvatars { get; }

    /// <summary>
    /// 玩家深渊信息
    /// </summary>
    public IEnumerable<PlayerSpiralAbyssLevel> PlayerSpiralAbyssesLevels { get; }

    /// <summary>
    /// 建造玩家记录
    /// </summary>
    /// <param name="uid">玩家的uid</param>
    /// <param name="detailAvatars">角色详情信息</param>
    /// <param name="spiralAbyss">深渊信息</param>
    /// <returns>玩家记录</returns>
    internal static PlayerRecord Create(string uid, List<Character> detailAvatars, SpiralAbyss spiralAbyss)
    {
        IEnumerable<PlayerAvatar> playerAvatars = detailAvatars
            .Select(avatar => new PlayerAvatar(avatar));

        IEnumerable<PlayerSpiralAbyssLevel> playerSpiralAbyssLevels = spiralAbyss.Floors
            .SelectMany(f => f.Levels, (f, level) => new IndexedLevel(f.Index, level))
            .Select(indexedLevel => new PlayerSpiralAbyssLevel(indexedLevel));

        PlayerRecord playerRecord = new(uid, playerAvatars, playerSpiralAbyssLevels);
        return playerRecord;
    }
}
