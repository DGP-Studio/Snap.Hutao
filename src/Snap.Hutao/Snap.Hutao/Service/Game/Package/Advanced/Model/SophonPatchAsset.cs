// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced.Model;

internal sealed class SophonPatchAsset
{
    public SophonPatchAsset(string urlPrefix, string urlSuffix, FileData fileData, PatchInfo patchInfo)
    {
        UrlPrefix = string.Intern(urlPrefix);
        UrlSuffix = string.Intern(urlSuffix);
        FileData = fileData;
        PatchInfo = patchInfo;
    }

    public string UrlPrefix { get; }

    public string UrlSuffix { get; }

    public FileData FileData { get; }

    public PatchInfo PatchInfo { get; }

    public string PatchDownloadUrl { get => $"{UrlPrefix}/{PatchInfo.Id}{UrlSuffix}"; }
}