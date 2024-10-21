// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

internal interface IPackageConverter
{
    ValueTask EnsureDeprecatedFilesAndSdkAsync(PackageConverterContext context);

    ValueTask<bool> EnsureGameResourceAsync(PackageConverterContext context);
}