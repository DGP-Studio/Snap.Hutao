// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced.Model;

internal sealed class SophonDecodedPatchManifest
{
    public SophonDecodedPatchManifest(string originalTag, string tag, string urlPrefix, string urlSuffix, PatchManifest data)
    {
        OriginalTag = originalTag;
        Tag = tag;
        UrlPrefix = urlPrefix;
        UrlSuffix = urlSuffix;
        Data = data;
    }

    public string OriginalTag { get; }

    public string Tag { get; }

    public string UrlPrefix { get; }

    public string UrlSuffix { get; }

    public PatchManifest Data { get; }
}