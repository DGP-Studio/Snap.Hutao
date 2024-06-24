// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Windowing;

internal sealed class XamlWindowNonRudeHWND : IDisposable
{
    // https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-itaskbarlist2-markfullscreenwindow#remarks
    private const string NonRudeHWND = "NonRudeHWND";
    private readonly HWND hwnd;

    [SuppressMessage("", "SH002")]
    public XamlWindowNonRudeHWND(HWND hwnd)
    {
        this.hwnd = hwnd;
        SetPropW(hwnd, NonRudeHWND, BOOL.TRUE);
    }

    public void Dispose()
    {
        RemovePropW(hwnd, NonRudeHWND);
    }
}