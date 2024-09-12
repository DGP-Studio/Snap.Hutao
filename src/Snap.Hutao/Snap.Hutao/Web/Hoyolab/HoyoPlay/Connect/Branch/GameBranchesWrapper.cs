// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;

internal sealed class GameBranchesWrapper
{
    [JsonPropertyName("game_branches")]
    public List<GameBranch> GameBranches { get; set; } = default!;
}
