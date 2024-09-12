// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Collections.Immutable;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Complex;

internal sealed partial class Team : List<AvatarView>
{
    public Team(ItemRate<string, int> team, ImmutableDictionary<AvatarId, Avatar> idAvatarMap, int rank)
        : base(4)
    {
        foreach (StringSegment item in new StringTokenizer(team.Item, [',']))
        {
            uint id = uint.Parse(item.AsSpan(), CultureInfo.InvariantCulture);
            Add(new(idAvatarMap[id], 0));
        }

        AddRange(new AvatarView[4 - Count]);

        UpCount = SH.FormatModelBindingHutaoTeamUpCountFormat(team.Rate);
        Rank = rank;
    }

    public string UpCount { get; }

    public int Rank { get; set; }
}