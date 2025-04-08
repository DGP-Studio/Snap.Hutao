// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;

internal interface IGamePackageOperation
{
    ValueTask ExecuteAsync(GamePackageServiceContext context);
}