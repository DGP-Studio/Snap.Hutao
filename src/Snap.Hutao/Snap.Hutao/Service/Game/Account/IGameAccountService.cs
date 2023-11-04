// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game.Account;

internal interface IGameAccountService
{
    ObservableCollection<GameAccount> GameAccountCollection { get; }

    void AttachGameAccountToUid(GameAccount gameAccount, string uid);

    GameAccount? DetectCurrentGameAccount();

    ValueTask<GameAccount?> DetectGameAccountAsync();

    ValueTask ModifyGameAccountAsync(GameAccount gameAccount);

    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    bool SetGameAccount(GameAccount account);
}