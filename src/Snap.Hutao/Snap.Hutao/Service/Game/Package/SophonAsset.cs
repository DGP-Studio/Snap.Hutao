// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package;

internal sealed class SophonAsset
{
    public SophonAsset(string urlPrefix, AssetProperty assetProperty)
    {
        UrlPrefix = urlPrefix;
        AssetProperty = assetProperty;
    }

    public SophonAsset(string urlPrefix, AssetProperty assetProperty, List<SophonChunk> diffChunks)
    {
        UrlPrefix = urlPrefix;
        AssetProperty = assetProperty;
        DiffChunks = diffChunks;
    }

    public string UrlPrefix { get; }

    public AssetProperty AssetProperty { get; }

    public List<SophonChunk> DiffChunks { get; } = default!;
}