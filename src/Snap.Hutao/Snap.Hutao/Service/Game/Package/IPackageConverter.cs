// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;

namespace Snap.Hutao.Service.Game.Package;

internal interface IPackageConverter
{
    ValueTask EnsureDeprecatedFilesAndSdkAsync(GameChannelSDK? channelSDK, DeprecatedFilesWrapper? deprecatedFiles, string gameFolder);

    ValueTask<bool> EnsureGameResourceAsync(LaunchScheme targetScheme, GamePackage gamePackage, string gameFolder, IProgress<PackageConvertStatus> progress);
}