// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.ViewModel.Complex;

/// <summary>
/// 队伍排行
/// </summary>
[HighQuality]
internal sealed class TeamAppearanceView
{
    /// <summary>
    /// 构造一个新的队伍排行
    /// </summary>
    /// <param name="teamRank">队伍排行</param>
    /// <param name="idAvatarMap">映射</param>
    public TeamAppearanceView(TeamAppearance teamRank, Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        Floor = SH.ModelBindingHutaoComplexRankFloor.Format(teamRank.Floor);
        Up = teamRank.Up.SelectList(teamRate => new Team(teamRate, idAvatarMap));
        Down = teamRank.Down.SelectList(teamRate => new Team(teamRate, idAvatarMap));
    }

    /// <summary>
    /// 层数
    /// </summary>
    public string Floor { get; }

    /// <summary>
    /// 上半阵容
    /// </summary>
    public List<Team> Up { get; }

    /// <summary>
    /// 下半阵容
    /// </summary>
    public List<Team> Down { get; }
}
