// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class SophonAssetOperation
{
    public SophonAssetOperationKind Kind { get; init; }

    public string UrlPrefix { get; init; } = default!;

    public AssetProperty OldAsset { get; init; } = default!;

    public AssetProperty NewAsset { get; init; } = default!;

    public List<SophonChunk> DiffChunks { get; init; } = [];

    public static SophonAssetOperation AddOrRepair(string urlPrefix, AssetProperty newAsset)
    {
        return new()
        {
            Kind = SophonAssetOperationKind.AddOrRepair,
            UrlPrefix = string.Intern(urlPrefix),
            NewAsset = newAsset,
            DiffChunks = newAsset.AssetChunks.Select(chunk => new SophonChunk(urlPrefix, chunk)).ToList(),
        };
    }

    public static SophonAssetOperation Modify(string urlPrefix, AssetProperty oldAsset, AssetProperty newAsset, List<SophonChunk> diffChunks)
    {
        return new()
        {
            Kind = SophonAssetOperationKind.Modify,
            UrlPrefix = string.Intern(urlPrefix),
            OldAsset = oldAsset,
            NewAsset = newAsset,
            DiffChunks = diffChunks,
        };
    }

    public static SophonAssetOperation Delete(AssetProperty oldAsset)
    {
        return new()
        {
            Kind = SophonAssetOperationKind.Delete,
            OldAsset = oldAsset,
        };
    }
}