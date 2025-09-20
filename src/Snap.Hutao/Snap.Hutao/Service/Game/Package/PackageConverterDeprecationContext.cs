// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Package;

internal readonly struct PackageConverterDeprecationContext
{
    public readonly HttpClient HttpClient;
    public readonly IGameFileSystemView GameFileSystem;
    public readonly GameChannelSDK? GameChannelSdk;
    public readonly DeprecatedFilesWrapper? DeprecatedFiles;

    public PackageConverterDeprecationContext(HttpClient httpClient, IGameFileSystemView gameFileSystem, GameChannelSDK? gameChannelSdk, DeprecatedFilesWrapper? deprecatedFiles)
    {
        HttpClient = httpClient;
        GameFileSystem = gameFileSystem;
        GameChannelSdk = gameChannelSdk;
        DeprecatedFiles = deprecatedFiles;
    }
}