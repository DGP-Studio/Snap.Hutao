// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Scheme;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game.Account;

internal interface IGameAccountService
{
    ObservableCollection<GameAccount> GameAccountCollection { get; }

    void AttachGameAccountToUid(GameAccount gameAccount, string uid);

    GameAccount? DetectCurrentGameAccount(LaunchScheme scheme);

    ValueTask<GameAccount?> DetectGameAccountAsync(LaunchScheme scheme);

    ValueTask ModifyGameAccountAsync(GameAccount gameAccount);

    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    bool SetGameAccount(GameAccount account);
}