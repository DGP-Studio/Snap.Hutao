// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class SophonAsset
{
    public SophonAsset(string urlPrefix, AssetProperty assetProperty)
    {
        UrlPrefix = string.Intern(urlPrefix);
        AssetProperty = assetProperty;
    }

    public string UrlPrefix { get; }

    public AssetProperty AssetProperty { get; }
}