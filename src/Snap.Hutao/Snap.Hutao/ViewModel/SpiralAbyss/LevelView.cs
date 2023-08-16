// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 间视图
/// </summary>
[HighQuality]
internal sealed class LevelView
{
    /// <summary>
    /// 构造一个新的间视图
    /// </summary>
    /// <param name="level">间</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    public LevelView(Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.Level level, Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap)
    {
        Index = SH.ModelBindingHutaoComplexRankLevel.Format(level.Index);
        Star = level.Star;
        Battles = level.Battles.SelectList(b => new BattleView(b, idAvatarMap));
    }

    /// <summary>
    /// 间号
    /// </summary>
    public string Index { get; }

    /// <summary>
    /// 星数
    /// </summary>
    public int Star { get; }

    /// <summary>
    /// 上下半
    /// </summary>
    public List<BattleView> Battles { get; }
}