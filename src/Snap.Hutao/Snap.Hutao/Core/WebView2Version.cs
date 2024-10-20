// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

internal readonly struct WebView2Version
{
    public readonly string Version;
    public readonly bool Supported;

    public WebView2Version(string version, bool supported)
    {
        Version = version;
        Supported = supported;
    }
}