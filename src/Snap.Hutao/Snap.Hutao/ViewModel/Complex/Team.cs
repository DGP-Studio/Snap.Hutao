// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Diagnostics;
using System.Globalization;

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
        foreach (StringSegment item in new StringTokenizer(team.Item, [',']))
        {
            uint id = uint.Parse(item.AsSpan(), CultureInfo.InvariantCulture);
            Add(new(idAvatarMap[id]));
        }

        AddRange(new AvatarView[4 - Count]);

        Rate = SH.FormatModelBindingHutaoTeamUpCountFormat(team.Rate);
    }

    /// <summary>
    /// 上场次数
    /// </summary>
    public string Rate { get; }
}