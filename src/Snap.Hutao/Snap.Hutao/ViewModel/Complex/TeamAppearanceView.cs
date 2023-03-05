// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.Model;

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
        Floor = string.Format(SH.ModelBindingHutaoComplexRankFloor, teamRank.Floor);
        Up = teamRank.Up.Select(teamRate => new Team(teamRate, idAvatarMap)).ToList();
        Down = teamRank.Down.Select(teamRate => new Team(teamRate, idAvatarMap)).ToList();
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
