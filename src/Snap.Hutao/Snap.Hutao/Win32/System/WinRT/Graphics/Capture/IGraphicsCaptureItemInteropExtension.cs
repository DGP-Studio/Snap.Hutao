// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Gdi;
using Windows.Graphics.Capture;

namespace Snap.Hutao.Win32.System.WinRT.Graphics.Capture;

internal static class IGraphicsCaptureItemInteropExtension
{
    public static unsafe HRESULT CreateForWindow(this IGraphicsCaptureItemInterop interop, HWND window, out GraphicsCaptureItem result)
    {
        void* resultPtr = default;
        HRESULT retVal;
        fixed (Guid* riid = &IGraphicsCaptureItem.IID)
        {
            retVal = interop.CreateForWindow(window, riid, &resultPtr);
        }

        result = GraphicsCaptureItem.FromAbi((nint)resultPtr);
        return retVal;
    }

    public static unsafe HRESULT CreateForMonitor(this IGraphicsCaptureItemInterop interop, HMONITOR monitor, out GraphicsCaptureItem result)
    {
        void* resultPtr = default;
        HRESULT retVal;
        fixed (Guid* riid2 = &IGraphicsCaptureItem.IID)
        {
            retVal = interop.CreateForMonitor(monitor, riid2, &resultPtr);
        }

        result = GraphicsCaptureItem.FromAbi((nint)resultPtr);
        return retVal;
    }
}