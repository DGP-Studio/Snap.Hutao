// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal readonly struct GamePackageOperationContext
{
    public readonly GamePackageOperationKind Kind;
    public readonly IGameAssetOperation Asset;
    public readonly IGameFileSystem GameFileSystem;
    public readonly SophonDecodedBuild LocalBuild;
    public readonly SophonDecodedBuild RemoteBuild;
    public readonly GameChannelSDK? GameChannelSDK;
    public readonly string EffectiveGameDirectory;
    public readonly string EffectiveChunksDirectory;

    public GamePackageOperationContext(
        IServiceProvider serviceProvider,
        GamePackageOperationKind kind,
        IGameFileSystem gameFileSystem,
        SophonDecodedBuild localBuild,
        SophonDecodedBuild remoteBuild,
        GameChannelSDK? gameChannelSDK,
        string? extractDirectory)
    {
        Kind = kind;
        Asset = serviceProvider.GetRequiredService<IDriverMediaTypeAwareFactory<IGameAssetOperation>>().Create(gameFileSystem.GetGameDirectory());
        GameFileSystem = gameFileSystem;
        LocalBuild = localBuild;
        RemoteBuild = remoteBuild;
        GameChannelSDK = gameChannelSDK;
        EffectiveGameDirectory = extractDirectory ?? gameFileSystem.GetGameDirectory();

        EffectiveChunksDirectory = kind is GamePackageOperationKind.Verify
            ? Path.Combine(gameFileSystem.GetChunksDirectory(), "repair")
            : gameFileSystem.GetChunksDirectory();
    }
}