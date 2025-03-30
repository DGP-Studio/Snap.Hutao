// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

internal readonly struct WebView2Version
{
    public readonly string RawVersion;
    public readonly string Version;
    public readonly bool Supported;

    public WebView2Version(string rawVersion, string version, bool supported)
    {
        RawVersion = rawVersion;
        Version = version;
        Supported = supported;
    }
}