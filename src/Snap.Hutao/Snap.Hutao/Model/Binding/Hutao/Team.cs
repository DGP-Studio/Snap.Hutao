// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
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
    public Team(ItemRate<string, int> team, Dictionary<AvatarId, Avatar> idAvatarMap)
        : base(4)
    {
        IOrderedEnumerable<int> ids = team.Item.Split(',').Select(int.Parse).OrderByDescending(x => x);

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