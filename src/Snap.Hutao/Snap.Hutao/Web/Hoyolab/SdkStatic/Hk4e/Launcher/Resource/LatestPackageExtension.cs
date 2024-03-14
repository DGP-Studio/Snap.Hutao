// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Resource;

internal static class LatestPackageExtension
{
    public static void Patch(this LatestPackage latest)
    {
        StringBuilder pathBuilder = new();
        foreach (PackageSegment segment in latest.Segments)
        {
            pathBuilder.AppendLine(segment.Path);
        }

        latest.Path = pathBuilder.ToStringTrimEndReturn();
        latest.Name = Path.GetFileName(latest.Segments[0].Path[..^4]); // .00X
    }
}