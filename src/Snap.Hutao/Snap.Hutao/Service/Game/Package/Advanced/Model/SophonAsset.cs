// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced.Model;

internal sealed class SophonAsset
{
    public SophonAsset(string urlPrefix, string urlSuffix, AssetProperty assetProperty)
    {
        UrlPrefix = string.Intern(urlPrefix);
        UrlSuffix = string.Intern(urlSuffix);
        AssetProperty = assetProperty;
    }

    public string UrlPrefix { get; }

    public string UrlSuffix { get; }

    public AssetProperty AssetProperty { get; }
}