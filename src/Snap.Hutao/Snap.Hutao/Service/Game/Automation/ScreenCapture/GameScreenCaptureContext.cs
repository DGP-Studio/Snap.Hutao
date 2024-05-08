// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Gdi;
using Snap.Hutao.Win32.System.WinRT.Graphics.Capture;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
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