// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dwm;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using Snap.Hutao.Win32.Graphics.Gdi;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.System.WinRT.Graphics.Capture;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.DwmApi;
using static Snap.Hutao.Win32.Gdi32;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal struct GameScreenCaptureContext : IDisposable
{
    public readonly GraphicsCaptureItem Item;
    public readonly bool PreviewEnabled;

    private const uint CreateDXGIFactoryFlag =
#if DEBUG
        DXGI_CREATE_FACTORY_DEBUG;
#else
        0;
#endif

    private const D3D11_CREATE_DEVICE_FLAG D3d11CreateDeviceFlag =
        D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT
#if DEBUG
        | D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG
#endif
        ;

    private readonly HWND hwnd;

    private unsafe IDXGIFactory6* factory;
    private unsafe IDXGIAdapter* adapter;
    private unsafe ID3D11Device* d3d11Device;
    private unsafe IDXGIDevice* dxgiDevice;
    private IDirect3DDevice? direct3DDevice;
    private unsafe IDXGISwapChain1* swapChain;

    [SuppressMessage("", "SH002")]
    private unsafe GameScreenCaptureContext(HWND hwnd, bool preview)
    {
        this.hwnd = hwnd;
        GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>().CreateForWindow(hwnd, out Item);

        PreviewEnabled = preview;
    }

    [SuppressMessage("", "SH002")]
    public static unsafe GameScreenCaptureContextCreationResult Create(HWND hwnd, bool preview)
    {
        GameScreenCaptureContext context = new(hwnd, preview);

        if (!DirectX.TryCreateDXGIFactory(CreateDXGIFactoryFlag, out context.factory, out HRESULT hr))
        {
            return new(GameScreenCaptureContextCreationResultKind.CreateDxgiFactoryFailed, hr);
        }

        if (!DirectX.TryGetHighPerformanceAdapter(context.factory, out context.adapter, out hr))
        {
            return new(GameScreenCaptureContextCreationResultKind.EnumAdapterByGpuPreferenceFailed, hr);
        }

        if (!DirectX.TryCreateD3D11Device(default, D3d11CreateDeviceFlag, out context.d3d11Device, out hr))
        {
            return new(GameScreenCaptureContextCreationResultKind.D3D11CreateDeviceFailed, hr);
        }

        if (!DirectX.TryAsDXGIDevice(context.d3d11Device, out context.dxgiDevice, out hr))
        {
            return new(GameScreenCaptureContextCreationResultKind.D3D11DeviceQueryDXGIDeviceFailed, hr);
        }

        if (!DirectX.TryCreateDirect3D11Device(context.dxgiDevice, out context.direct3DDevice, out hr))
        {
            return new(GameScreenCaptureContextCreationResultKind.CreateDirect3D11DeviceFromDXGIDeviceFailed, hr);
        }

        return new GameScreenCaptureContextCreationResult(GameScreenCaptureContextCreationResultKind.Success, context);
    }

    public Direct3D11CaptureFramePool CreatePool()
    {
        (DirectXPixelFormat winrt, DXGI_FORMAT dx) = DeterminePixelFormat(hwnd);
        CreateOrUpdateDXGISwapChain(dx);
        return Direct3D11CaptureFramePool.CreateFreeThreaded(direct3DDevice, winrt, 2, Item.Size);
    }

    public void RecreatePool(Direct3D11CaptureFramePool framePool)
    {
        (DirectXPixelFormat winrt, DXGI_FORMAT dx) = DeterminePixelFormat(hwnd);
        CreateOrUpdateDXGISwapChain(dx);
        framePool.Recreate(direct3DDevice, winrt, 2, Item.Size);
    }

    public readonly GraphicsCaptureSession CreateSession(Direct3D11CaptureFramePool framePool)
    {
        GraphicsCaptureSession session = framePool.CreateCaptureSession(Item);
        session.IsCursorCaptureEnabled = false;
        session.IsBorderRequired = false;
        return session;
    }

    public readonly bool TryGetClientBox(uint width, uint height, out D3D11_BOX clientBox)
    {
        clientBox = default;

        // Ensure the window is not minimized
        if (IsIconic(hwnd))
        {
            return false;
        }

        // Ensure the window is at least partially in the screen
        if (!(GetClientRect(hwnd, out RECT clientRect) && (clientRect.right > 0) && (clientRect.bottom > 0)))
        {
            return false;
        }

        // Ensure we get the window chrome rect
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

    public unsafe readonly void AttachPreview(GameScreenCaptureDebugPreviewWindow? window)
    {
        if (PreviewEnabled && window is not null)
        {
            window.UpdateSwapChain(swapChain);
        }
    }

    public unsafe readonly void UpdatePreview(GameScreenCaptureDebugPreviewWindow? window, IDirect3DSurface surface)
    {
        if (PreviewEnabled && window is not null)
        {
            window.UnsafeUpdatePreview(d3d11Device, surface);
        }
    }

    public unsafe readonly void DetachPreview(GameScreenCaptureDebugPreviewWindow? window)
    {
        if (PreviewEnabled && window is not null)
        {
            window.UpdateSwapChain(null);
            window.Close();
        }
    }

    public unsafe readonly void Dispose()
    {
        IUnknownMarshal.Release(factory);
        IUnknownMarshal.Release(swapChain);
    }

    [SuppressMessage("", "SH002")]
    private static (DirectXPixelFormat WinRTFormat, DXGI_FORMAT DXFormat) DeterminePixelFormat(HWND hwnd)
    {
        HDC hdc = GetDC(hwnd);
        if (hdc != HDC.NULL)
        {
            int bitsPerPixel = GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.BITSPIXEL);
            _ = ReleaseDC(hwnd, hdc);
            if (bitsPerPixel >= 32)
            {
                return (DirectXPixelFormat.R16G16B16A16Float, DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT);
            }
        }

        return (DirectXPixelFormat.B8G8R8A8UIntNormalized, DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM);
    }

    private unsafe void CreateOrUpdateDXGISwapChain(DXGI_FORMAT format)
    {
        if (!PreviewEnabled)
        {
            return;
        }

        DXGI_SWAP_CHAIN_DESC1 desc = default;
        desc.Width = (uint)Item.Size.Width;
        desc.Height = (uint)Item.Size.Height;
        desc.Format = format;
        desc.SampleDesc.Count = 1;
        desc.BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT;
        desc.BufferCount = 2;
        desc.Scaling = DXGI_SCALING.DXGI_SCALING_STRETCH;
        desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;
        desc.AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_PREMULTIPLIED;

        DirectX.TryCreateSwapChainForComposition(factory, d3d11Device, in desc, out swapChain, out HRESULT hr);
    }
}