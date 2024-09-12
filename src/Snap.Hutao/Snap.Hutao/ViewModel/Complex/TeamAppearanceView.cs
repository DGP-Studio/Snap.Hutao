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
        Up = teamRank.Up.SelectList((teamRate, index) => new Team(teamRate, idAvatarMap, index + 1));
        Down = teamRank.Down.SelectList((teamRate, index) => new Team(teamRate, idAvatarMap, index + 1));
    }

    public string Floor { get; }

    public List<Team> Up { get; }

    public List<Team> Down { get; }
}
