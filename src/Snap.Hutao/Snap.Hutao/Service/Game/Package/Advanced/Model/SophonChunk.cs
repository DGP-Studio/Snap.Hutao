// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced.Model;

internal sealed class SophonChunk
{
    public SophonChunk(string urlPrefix, string urlSuffix, AssetChunk assetChunk)
    {
        UrlPrefix = string.Intern(urlPrefix);
        UrlSuffix = string.Intern(urlSuffix);
        AssetChunk = assetChunk;
    }

    public string UrlPrefix { get; }

    public string UrlSuffix { get; }

    public AssetChunk AssetChunk { get; }

    public string ChunkDownloadUrl { get => $"{UrlPrefix}/{AssetChunk.ChunkName}{UrlSuffix}"; }
}