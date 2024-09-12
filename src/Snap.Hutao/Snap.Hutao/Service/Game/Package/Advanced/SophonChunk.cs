﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class SophonChunk
{
    public SophonChunk(string urlPrefix, AssetChunk assetChunk)
    {
        UrlPrefix = string.Intern(urlPrefix);
        AssetChunk = assetChunk;
    }

    public string UrlPrefix { get; }

    public AssetChunk AssetChunk { get; }

    public string ChunkDownloadUrl { get => $"{UrlPrefix}/{AssetChunk.ChunkName}"; }
}