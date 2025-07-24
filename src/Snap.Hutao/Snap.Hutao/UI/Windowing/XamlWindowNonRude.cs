// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

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
        try
        {
            native.Detach();
        }
        catch (COMException ex)
        {
            if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_MAPPED_ALIGNMENT))
            {
                return;
            }

            throw;
        }
    }
}