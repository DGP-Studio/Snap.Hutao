// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package.Advanced.AssetOperation;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class GamePackageOperationContext
{
    public GamePackageOperationContext(IServiceProvider serviceProvider, GamePackageOperationKind kind, IGameFileSystem gameFileSystem, string? extractDirectory = default)
    {
        Kind = kind;
        Asset = serviceProvider.GetRequiredService<IDriverMediaTypeAwareFactory<IGameAssetOperation>>().Create(string.Empty);
        GameFileSystem = gameFileSystem;

        EffectiveGameDirectory = extractDirectory ?? gameFileSystem.GetGameDirectory();

        EffectiveChunksDirectory = kind is GamePackageOperationKind.Verify
            ? Path.Combine(gameFileSystem.GetChunksDirectory(), "repair")
            : gameFileSystem.GetChunksDirectory();
    }

    public GamePackageOperationKind Kind { get; }

    public IGameAssetOperation Asset { get; }

    public IGameFileSystem GameFileSystem { get; init; }

    public SophonDecodedBuild? LocalBuild { get; init; }

    public SophonDecodedBuild? RemoteBuild { get; init; }

    public SophonDecodedPatchBuild? PatchBuild { get; init; }

    public GameChannelSDK? GameChannelSDK { get; init; }

    public string EffectiveGameDirectory { get; }

    public string EffectiveChunksDirectory { get; }
}