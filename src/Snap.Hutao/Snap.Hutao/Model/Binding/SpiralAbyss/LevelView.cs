// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.Model.Binding.SpiralAbyss;

/// <summary>
/// 间视图
/// </summary>
public class LevelView
{
    /// <summary>
    /// 构造一个新的间视图
    /// </summary>
    /// <param name="level">间</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    public LevelView(Level level, Dictionary<AvatarId, Metadata.Avatar.Avatar> idAvatarMap)
    {
        Index = $"第 {level.Index} 间";
        Star = level.Star;
        Battles = level.Battles.Select(b => new BattleView(b, idAvatarMap)).ToList();
    }

    /// <summary>
    /// 间号
    /// </summary>
    public string Index { get; set; }

    /// <summary>
    /// 星数
    /// </summary>
    public int Star { get; set; }

    /// <summary>
    /// 上下半
    /// </summary>
    public List<BattleView> Battles { get; set; }
}
