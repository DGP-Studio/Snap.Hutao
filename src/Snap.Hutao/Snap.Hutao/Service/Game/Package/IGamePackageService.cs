// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.Service.Game.Package;

internal interface IGamePackageService
{
    ValueTask<bool> EnsureGameResourceAsync(LaunchScheme launchScheme, IProgress<PackageReplaceStatus> progress);
}