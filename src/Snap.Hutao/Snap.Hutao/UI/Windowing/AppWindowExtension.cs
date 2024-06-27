// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Snap.Hutao.Core.Graphics;
using Windows.Graphics;

namespace Snap.Hutao.UI.Windowing;

[HighQuality]
internal static class AppWindowExtension
{
    public static unsafe RectInt32 GetRect(this AppWindow appWindow)
    {
        RectInt32View view = default;
        view.Position = appWindow.Position;
        view.Size = appWindow.Size;
        return *(RectInt32*)&view;
    }

    public static unsafe void MoveThenResize(this AppWindow appWindow, RectInt32 rectInt32)
    {
        RectInt32View* pView = (RectInt32View*)&rectInt32;
        appWindow.Move(pView->Position);
        appWindow.Resize(pView->Size);
    }
}