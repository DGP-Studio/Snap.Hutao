// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Package.Advanced.Model;

internal sealed class SophonAssetOperation
{
    private SophonAssetOperation()
    {
    }

    public SophonAssetOperationKind Kind { get; private init; }

    public string UrlPrefix { get; private init; } = default!;

    public string UrlSuffix { get; private init; } = default!;

    public AssetProperty OldAsset { get; private init; } = default!;

    public AssetProperty NewAsset { get; private init; } = default!;

    public ImmutableArray<SophonChunk> DiffChunks { get; private init; } = [];

    public static SophonAssetOperation AddOrRepair(string urlPrefix, string urlSuffix, AssetProperty newAsset)
    {
        return new()
        {
            Kind = SophonAssetOperationKind.AddOrRepair,
            UrlPrefix = string.Intern(urlPrefix),
            UrlSuffix = string.Intern(urlSuffix),
            NewAsset = newAsset,
            DiffChunks = [.. newAsset.AssetChunks.Select(chunk => new SophonChunk(urlPrefix, urlSuffix, chunk))],
        };
    }

    public static SophonAssetOperation Modify(string urlPrefix, string urlSuffix, AssetProperty oldAsset, AssetProperty newAsset, ImmutableArray<SophonChunk> diffChunks)
    {
        return new()
        {
            Kind = SophonAssetOperationKind.Modify,
            UrlPrefix = string.Intern(urlPrefix),
            UrlSuffix = string.Intern(urlSuffix),
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