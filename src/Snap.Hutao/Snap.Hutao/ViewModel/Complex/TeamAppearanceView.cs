// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Complex;

internal sealed class TeamAppearanceView
{
    public TeamAppearanceView(TeamAppearance teamRank, ImmutableDictionary<AvatarId, Avatar> idAvatarMap)
    {
        Floor = SH.FormatModelBindingHutaoComplexRankFloor(teamRank.Floor);
        Up = teamRank.Up.SelectAsArray(static (teamRate, index, idAvatarMap) => new Team(teamRate, idAvatarMap, index + 1), idAvatarMap);
        Down = teamRank.Down.SelectAsArray(static (teamRate, index, idAvatarMap) => new Team(teamRate, idAvatarMap, index + 1), idAvatarMap);
    }

    public string Floor { get; }

    public ImmutableArray<Team> Up { get; }

    public ImmutableArray<Team> Down { get; }
}