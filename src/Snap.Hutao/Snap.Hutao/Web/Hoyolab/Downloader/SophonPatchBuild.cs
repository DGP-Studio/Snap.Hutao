// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Downloader;

internal sealed class SophonPatchBuild
{
    [JsonPropertyName("build_id")]
    public string BuildId { get; set; } = default!;

    [JsonPropertyName("patch_id")]
    public string PatchId { get; set; } = default!;

    [JsonPropertyName("tag")]
    public string Tag { get; set; } = default!;

    [JsonPropertyName("manifests")]
    public List<SophonPatchManifest> Manifests { get; set; } = default!;
}