// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.Model;

namespace Snap.Hutao.ViewModel.Complex;

/// <summary>
/// 队伍
/// </summary>
[HighQuality]
internal sealed class Team : List<AvatarView>
{
    /// <summary>
    /// 构造一个新的队伍
    /// </summary>
    /// <param name="team">队伍</param>
    /// <param name="idAvatarMap">映射</param>
    public Team(ItemRate<string, int> team, Dictionary<AvatarId, Avatar> idAvatarMap)
        : base(4)
    {
        foreach (StringSegment item in new StringTokenizer(team.Item, new char[] { ',' }))
        {
            int id = int.Parse(item.AsSpan());
            Add(new(idAvatarMap[id], 0));
        }

        Rate = string.Format(SH.ModelBindingHutaoTeamUpCountFormat, team.Rate);
    }

    /// <summary>
    /// 上场次数
    /// </summary>
    public string Rate { get; }
}