// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal interface IGamePackageService
{
    ValueTask<bool> ExecuteOperationAsync(GamePackageOperationContext context);

    ValueTask CancelOperationAsync();
}
