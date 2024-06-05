// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.System.Com;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.D3d11;
using static Snap.Hutao.Win32.Dxgi;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal static class DirectX
{
    public static unsafe bool TryCreateDXGIFactory(uint flags, out IDXGIFactory6* factory, out HRESULT hr)
    {
        hr = CreateDXGIFactory2(flags, in IDXGIFactory6.IID, out factory);
        return SUCCEEDED(hr);
    }

    public static unsafe bool TryGetHighPerformanceAdapter(IDXGIFactory6* factory, out IDXGIAdapter* adapter, out HRESULT hr)
    {
        hr = factory->EnumAdapterByGpuPreference(0U, DXGI_GPU_PREFERENCE.DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, in IDXGIAdapter.IID, out adapter);
        if (FAILED(hr))
        {
            return false;
        }

        IUnknownMarshal.Release(adapter);
        return true;
    }

    public static unsafe bool TryCreateD3D11Device(IDXGIAdapter* adapter, D3D11_CREATE_DEVICE_FLAG flags, out ID3D11Device* device, out HRESULT hr)
    {
        hr = D3D11CreateDevice(adapter, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, default, flags, [], D3D11_SDK_VERSION, out device, out _, out _);
        if (FAILED(hr))
        {
            return false;
        }

        IUnknownMarshal.Release(device);
        return true;
    }

    public static unsafe bool TryAsDXGIDevice(ID3D11Device* device, out IDXGIDevice* dxgiDevice, out HRESULT hr)
    {
        hr = IUnknownMarshal.QueryInterface(device, in IDXGIDevice.IID, out dxgiDevice);
        if (FAILED(hr))
        {
            return false;
        }

        IUnknownMarshal.Release(dxgiDevice);
        return true;
    }

    public static unsafe bool TryCreateDirect3D11Device(IDXGIDevice* dxgiDevice, [NotNullWhen(true)] out IDirect3DDevice? direct3DDevice, out HRESULT hr)
    {
        hr = CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice, out Win32.System.WinRT.IInspectable* inspectable);
        if (FAILED(hr))
        {
            direct3DDevice = default;
            return false;
        }

        direct3DDevice = IInspectable.FromAbi((nint)inspectable).ObjRef.AsInterface<IDirect3DDevice>();
        return true;
    }

    public static unsafe bool TryCreateSwapChainForComposition(IDXGIFactory6* factory, ID3D11Device* device, ref readonly DXGI_SWAP_CHAIN_DESC1 desc, out IDXGISwapChain1* swapChain, out HRESULT hr)
    {
        hr = factory->CreateSwapChainForComposition((IUnknown*)device, in desc, default, out swapChain);
        if (FAILED(hr))
        {
            return false;
        }

        return true;
    }
}