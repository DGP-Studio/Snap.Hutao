// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Snap.Hutao.Win32;
using Windows.Graphics;

namespace Snap.Hutao.UI.Windowing;

[HighQuality]
internal static class AppWindowExtension
{
    public static RectInt32 GetRect(this AppWindow appWindow)
    {
        return StructMarshal.RectInt32(appWindow.Position, appWindow.Size);
    }
}