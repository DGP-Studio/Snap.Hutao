// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class GameIdentity
{
    public UserAndUid? UserAndUid { get; init; }

    public GameAccount? GameAccount { get; init; }

    public static GameIdentity Create(UserAndUid? userAndUid, GameAccount? gameAccount)
    {
        return new()
        {
            UserAndUid = userAndUid,
            GameAccount = gameAccount,
        };
    }

    public static GameIdentity Create()
    {
        return new();
    }
}