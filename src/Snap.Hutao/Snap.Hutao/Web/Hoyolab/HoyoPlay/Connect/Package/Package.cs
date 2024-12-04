// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;

internal sealed class Package
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = default!;

    [JsonPropertyName("game_pkgs")]
    public List<PackageSegment> GamePackages { get; set; } = default!;

    [JsonPropertyName("audio_pkgs")]
    public List<AudioPackageSegment> AudioPackages { get; set; } = default!;

    [JsonPropertyName("res_list_url")]
    public string ResourceListUrl { get; set; } = default!;

    [JsonIgnore]
    public List<PackageSegment> AllPackages
    {
        get => [.. GamePackages, .. AudioPackages];
    }
}
