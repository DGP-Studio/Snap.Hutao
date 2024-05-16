// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dwm;
using Snap.Hutao.Win32.Graphics.Gdi;
using Snap.Hutao.Win32.System.WinRT.Graphics.Capture;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using static Snap.Hutao.Win32.DwmApi;
using static Snap.Hutao.Win32.Gdi32;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal readonly struct GameScreenCaptureContext
{
    public readonly GraphicsCaptureItem Item;

    private readonly IDirect3DDevice direct3DDevice;
    private readonly HWND hwnd;

    public GameScreenCaptureContext(IDirect3DDevice direct3DDevice, HWND hwnd)
    {
        this.direct3DDevice = direct3DDevice;
        this.hwnd = hwnd;

        GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>().CreateForWindow(hwnd, out Item);
    }

    public Direct3D11CaptureFramePool CreatePool()
    {
        return Direct3D11CaptureFramePool.CreateFreeThreaded(direct3DDevice, DeterminePixelFormat(hwnd), 2, Item.Size);
    }

    public void RecreatePool(Direct3D11CaptureFramePool framePool)
    {
        framePool.Recreate(direct3DDevice, DeterminePixelFormat(hwnd), 2, Item.Size);
    }

    public GraphicsCaptureSession CreateSession(Direct3D11CaptureFramePool framePool)
    {
        GraphicsCaptureSession session = framePool.CreateCaptureSession(Item);
        session.IsCursorCaptureEnabled = false;
        session.IsBorderRequired = false;
        return session;
    }

    public bool TryGetClientBox(uint width, uint height, out D3D11_BOX clientBox)
    {
        clientBox = default;

        // Check if the window is minimized
        if (IsIconic(hwnd))
        {
            return false;
        }

        // Check if the window is at least partially in the screen
        if (!(GetClientRect(hwnd, out RECT clientRect) && (clientRect.right > 0) && (clientRect.bottom > 0)))
        {
            return false;
        }

        // Make sure we get the window chrome rect
        if (DwmGetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out RECT windowRect) != HRESULT.S_OK)
        {
            return false;
        }

        // Provide a client side (0, 0) and translate to screen coordinates
        POINT clientPoint = default;
        if (!ClientToScreen(hwnd, ref clientPoint))
        {
            return false;
        }

        uint left = clientBox.left = clientPoint.x > windowRect.left ? (uint)(clientPoint.x - windowRect.left) : 0U;
        uint top = clientBox.top = clientPoint.y > windowRect.top ? (uint)(clientPoint.y - windowRect.top) : 0U;
        clientBox.right = left + (width > left ? (uint)Math.Min(width - left, clientRect.right) : 1U);
        clientBox.bottom = top + (height > top ? (uint)Math.Min(height - top, clientRect.bottom) : 1U);
        clientBox.front = 0U;
        clientBox.back = 1U;

        return clientBox.right <= width && clientBox.bottom <= height;
    }

    private static DirectXPixelFormat DeterminePixelFormat(HWND hwnd)
    {
        HDC hdc = GetDC(hwnd);
        if (hdc != HDC.NULL)
        {
            int bitsPerPixel = GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.BITSPIXEL);
            _ = ReleaseDC(hwnd, hdc);
            if (bitsPerPixel >= 32)
            {
                return DirectXPixelFormat.R16G16B16A16Float;
            }
        }

        return DirectXPixelFormat.B8G8R8A8UIntNormalized;
    }
}