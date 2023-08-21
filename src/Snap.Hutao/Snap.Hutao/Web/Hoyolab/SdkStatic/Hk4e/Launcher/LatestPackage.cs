// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

internal sealed class LatestPackage : Package
{
    [JsonPropertyName("segments")]
    public List<PackageSegment> Segments { get; set; } = default!;
}