// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Service.Game.Account;

internal interface IGameAccountService
{
    GameAccount? DetectCurrentGameAccount(SchemeType schemeType);

    ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType schemeType);

    ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync();

    ValueTask ModifyGameAccountAsync(GameAccount gameAccount);

    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    bool SetGameAccount(GameAccount account);
}