// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

[SuppressMessage("", "SA1307")]
internal struct WNDCLASSW
{
    public WNDCLASS_STYLES style;
    public WNDPROC lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public HINSTANCE hInstance;
    public HICON hIcon;
    public HCURSOR hCursor;
    public HBRUSH hbrBackground;
    public PCWSTR lpszMenuName;
    public PCWSTR lpszClassName;
}