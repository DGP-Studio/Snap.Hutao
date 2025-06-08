// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.UI.Windowing;

internal sealed partial class XamlWindowNonRude : IDisposable
{
    private readonly HutaoNativeWindowNonRude native;

    public XamlWindowNonRude(HWND hwnd)
    {
        native = HutaoNative.Instance.MakeWindowNonRude(hwnd);
        native.Attach();
    }

    public void Dispose()
    {
        native.Detach();
    }
}