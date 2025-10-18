// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Service.Game.Account;

internal interface IGameInRegistryAccountService
{
    GameAccount? DetectCurrentGameAccount(SchemeType schemeType);

    ValueTask<GameAccount?> DetectCurrentGameAccountAsync(SchemeType schemeType, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback);

    ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync();

    ValueTask ModifyGameAccountAsync(GameAccount gameAccount, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback);

    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    bool SetCurrentGameAccount(GameAccount account);
}