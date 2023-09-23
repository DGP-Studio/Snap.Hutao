// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32.Foundation;

namespace Snap.Hutao.Core.Windowing.HotKey;

internal interface IHotKeyController
{
    void OnHotKeyPressed(in HotKeyParameter parameter);

    bool Register(in HWND hwnd);

    bool Unregister(in HWND hwnd);
}