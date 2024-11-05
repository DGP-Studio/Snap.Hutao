// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package;

internal sealed class PackageItemOperationForSophonChunks
{
    public PackageItemOperationKind Kind { get; init; }

    public string UrlPrefix { get; init; } = default!;

    public AssetProperty OldAsset { get; init; } = default!;

    public AssetProperty NewAsset { get; init; } = default!;

    public List<SophonChunk> DiffChunks { get; init; } = [];

    public static PackageItemOperationForSophonChunks Add(string urlPrefix, AssetProperty newAsset)
    {
        return new()
        {
            Kind = PackageItemOperationKind.Add,
            UrlPrefix = string.Intern(urlPrefix),
            NewAsset = newAsset,
            DiffChunks = newAsset.AssetChunks.Select(chunk => new SophonChunk(urlPrefix, chunk)).ToList(),
        };
    }

    public static PackageItemOperationForSophonChunks ModifyOrReplace(string urlPrefix, AssetProperty oldAsset, AssetProperty newAsset, List<SophonChunk> diffChunks)
    {
        return new()
        {
            Kind = PackageItemOperationKind.Replace,
            UrlPrefix = string.Intern(urlPrefix),
            OldAsset = oldAsset,
            NewAsset = newAsset,
            DiffChunks = diffChunks,
        };
    }

    public static PackageItemOperationForSophonChunks Backup(AssetProperty oldAsset)
    {
        return new()
        {
            Kind = PackageItemOperationKind.Backup,
            OldAsset = oldAsset,
        };
    }
}