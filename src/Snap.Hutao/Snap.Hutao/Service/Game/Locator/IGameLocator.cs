// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Locator;

internal interface IGameLocator
{
    ValueTask<ValueResult<bool, string>> LocateSingleGamePathAsync();
}