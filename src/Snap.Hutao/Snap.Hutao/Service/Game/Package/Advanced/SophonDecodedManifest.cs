// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class SophonDecodedManifest
{
    public SophonDecodedManifest(string urlPrefix, SophonManifestProto sophonData)
    {
        UrlPrefix = string.Intern(urlPrefix);
        Data = sophonData;
    }

    public string UrlPrefix { get; }

    public SophonManifestProto Data { get; }
}