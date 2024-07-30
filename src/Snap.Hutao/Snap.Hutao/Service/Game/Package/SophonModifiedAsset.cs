// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package;

internal sealed class SophonModifiedAsset
{
    public SophonModifiedAsset(string urlPrefix, AssetProperty oldAsset, AssetProperty newAsset, List<SophonChunk> diffChunks)
    {
        UrlPrefix = string.Intern(urlPrefix);
        OldAsset = oldAsset;
        NewAsset = newAsset;
        DiffChunks = diffChunks;
    }

    public string UrlPrefix { get; }

    public AssetProperty OldAsset { get; }

    public AssetProperty NewAsset { get; }

    public List<SophonChunk> DiffChunks { get; }
}