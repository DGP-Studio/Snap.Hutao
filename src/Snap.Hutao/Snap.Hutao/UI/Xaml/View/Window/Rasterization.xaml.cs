// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

internal static class ScaledSizeInt32
{
    public static SizeInt32 CreateForWindow(int width, int height, Microsoft.UI.Xaml.Window window)
    {
        return new SizeInt32(width, height).Scale(window.GetRasterizationScale());
    }
}