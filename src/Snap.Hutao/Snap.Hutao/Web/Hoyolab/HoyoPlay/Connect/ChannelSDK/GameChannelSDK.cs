// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;

internal sealed class GameChannelSDK : GameIndexedObject
{
    [JsonPropertyName("channel_sdk_pkg")]
    public PackageSegment ChannelSdkPackage { get; set; } = default!;

    [JsonPropertyName("pkg_version_file_name")]
    public string PackageVersionFileName { get; set; } = default!;

    [JsonPropertyName("version")]
    public string Version { get; set; } = default!;
}
