// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal interface IGamePackageService
{
    ValueTask<bool> StartOperationAsync(GamePackageOperationContext context);

    ValueTask CancelOperationAsync();
}
