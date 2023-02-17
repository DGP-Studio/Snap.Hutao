// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Binding.SpiralAbyss;

/// <summary>
/// 层视图
/// </summary>
[HighQuality]
internal sealed class FloorView
{
    /// <summary>
    /// 构造一个新的层视图
    /// </summary>
    /// <param name="floor">层</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    public FloorView(Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.Floor floor, Dictionary<AvatarId, Metadata.Avatar.Avatar> idAvatarMap)
    {
        Index = string.Format(SH.ModelBindingHutaoComplexRankFloor, floor.Index);
        SettleTime = $"{DateTimeOffset.FromUnixTimeSeconds(floor.SettleTime).ToLocalTime():yyyy.MM.dd HH:mm:ss}";
        Star = floor.Star;
        Levels = floor.Levels.Select(l => new LevelView(l, idAvatarMap)).ToList();
    }

    /// <summary>
    /// 层号
    /// </summary>
    public string Index { get; }

    /// <summary>
    /// 时间
    /// </summary>
    public string SettleTime { get; }

    /// <summary>
    /// 星数
    /// </summary>
    public int Star { get; }

    /// <summary>
    /// 间信息
    /// </summary>
    public List<LevelView> Levels { get; }
}