// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 玩家记录
/// 使用 <see cref="CreateAsync(string, List{Character}, SpiralAbyss)"/> 来构建一个实例
/// </summary>
public class PlayerRecord
{
    /// <summary>
    /// 防止从外部构造一个新的玩家记录
    /// </summary>
    private PlayerRecord()
    {
    }

    /// <summary>
    /// uid
    /// </summary>
    public string Uid { get; private set; } = default!;

    /// <summary>
    /// 玩家角色
    /// </summary>
    public IEnumerable<PlayerAvatar> PlayerAvatars { get; private set; } = default!;

    /// <summary>
    /// 玩家深渊信息
    /// </summary>
    public IEnumerable<PlayerSpiralAbyssLevel> PlayerSpiralAbyssesLevels { get; private set; } = default!;

    /// <summary>
    /// 造成最多伤害
    /// </summary>
    public Damage? DamageMost { get; private set; }

    /// <summary>
    /// 承受最多伤害
    /// </summary>
    public Damage? TakeDamageMost { get; private set; }

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

        return new()
        {
            Uid = uid,
            PlayerAvatars = playerAvatars,
            PlayerSpiralAbyssesLevels = playerSpiralAbyssLevels,
            DamageMost = GetDamage(spiralAbyss.DamageRank),
            TakeDamageMost = GetDamage(spiralAbyss.TakeDamageRank),
        };
    }

    /// <summary>
    /// 上传记录
    /// </summary>
    /// <param name="hutaoClient">使用的客户端</param>
    /// <param name="token">取消令牌</param>
    /// <returns>上传结果</returns>
    internal Task<Response<string>?> UploadAsync(HutaoClient hutaoClient, CancellationToken token = default)
    {
        return hutaoClient.UploadRecordAsync(this, token);
    }

    private static Damage? GetDamage(List<RankInfo> ranks)
    {
        if (ranks.Count > 0)
        {
            RankInfo rank = ranks[0];
            return new Damage(rank.AvatarId, rank.Value);
        }

        return null;
    }
}
