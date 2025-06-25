// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced.Model;

internal sealed class SophonDecodedPatchManifest
{
    public SophonDecodedPatchManifest(string urlPrefix, string urlSuffix, PatchManifest patchManifest)
    {
        UrlPrefix = string.Intern(urlPrefix);
        UrlSuffix = string.IsNullOrEmpty(urlSuffix) ? string.Empty : string.Intern($"?{urlSuffix}");
        Data = patchManifest;
    }

    public string UrlPrefix { get; }

    public string UrlSuffix { get; }

    public PatchManifest Data { get; }
}