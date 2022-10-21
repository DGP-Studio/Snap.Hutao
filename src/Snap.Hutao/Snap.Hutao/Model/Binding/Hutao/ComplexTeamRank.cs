// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Web.Hutao.Model;

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 队伍排行
/// </summary>
internal class ComplexTeamRank
{
    /// <summary>
    /// 构造一个新的队伍排行
    /// </summary>
    /// <param name="teamRank">队伍排行</param>
    /// <param name="idAvatarMap">映射</param>
    public ComplexTeamRank(TeamAppearance teamRank, Dictionary<int, Avatar> idAvatarMap)
    {
        Floor = $"第 {teamRank.Floor} 层";
        Up = teamRank.Up.Select(teamRate => new Team(teamRate, idAvatarMap)).ToList();
        Down = teamRank.Down.Select(teamRate => new Team(teamRate, idAvatarMap)).ToList();
    }

    /// <summary>
    /// 层数
    /// </summary>
    public string Floor { get; set; } = default!;

    /// <summary>
    /// 上半阵容
    /// </summary>
    public List<Team> Up { get; set; } = default!;

    /// <summary>
    /// 下半阵容
    /// </summary>
    public List<Team> Down { get; set; } = default!;
}
