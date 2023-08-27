// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

internal sealed class PackageSegment : PathMd5
{
    [JsonPropertyName("package_size")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long PackageSize { get; set; } = default!;
}