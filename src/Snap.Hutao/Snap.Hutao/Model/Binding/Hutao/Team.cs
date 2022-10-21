// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Web.Hutao.Model;

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 队伍
/// </summary>
internal class Team : List<ComplexAvatar>
{
    /// <summary>
    /// 构造一个新的队伍
    /// </summary>
    /// <param name="team">队伍</param>
    /// <param name="idAvatarMap">映射</param>
    public Team(ItemRate<string, int> team, Dictionary<int, Avatar> idAvatarMap)
        : base(4)
    {
        IEnumerable<int> ids = team.Item.Split(',').Select(i => int.Parse(i));

        foreach (int id in ids)
        {
            Add(new(idAvatarMap[id], 0));
        }

        Rate = $"上场 {team.Rate} 次";
    }

    /// <summary>
    /// 上场次数
    /// </summary>
    public string Rate { get; set; }
}