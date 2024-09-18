// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.D3d11;
using static Snap.Hutao.Win32.Dxgi;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal static class DirectX
{
    public static unsafe bool TryCreateDXGIFactory(uint flags, out ObjectReference<IDXGIFactory6.Vftbl> factory, out HRESULT hr)
    {
        hr = CreateDXGIFactory2(flags, in IDXGIFactory6.IID, out factory);
        return SUCCEEDED(hr);
    }

    public static unsafe bool TryGetHighPerformanceAdapter(ObjectReference<IDXGIFactory6.Vftbl> factory, out ObjectReference<IDXGIAdapter.Vftbl> adapter, out HRESULT hr)
    {
        hr = ((IDXGIFactory6*)factory.ThisPtr)->EnumAdapterByGpuPreference(0U, DXGI_GPU_PREFERENCE.DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, in IDXGIAdapter.IID, out adapter);
        return SUCCEEDED(hr);
    }

    public static unsafe bool TryCreateD3D11Device(ObjectReference<IDXGIAdapter.Vftbl> adapter, D3D11_CREATE_DEVICE_FLAG flags, out ObjectReference<ID3D11Device.Vftbl> device, out HRESULT hr)
    {
        hr = D3D11CreateDevice(adapter, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, default, flags, [], D3D11_SDK_VERSION, out device, out _, out _);
        return SUCCEEDED(hr);
    }

    public static unsafe bool TryCreateDirect3D11Device(ObjectReference<IDXGIDevice.Vftbl> dxgiDevice, [NotNullWhen(true)] out IDirect3DDevice? direct3DDevice, out HRESULT hr)
    {
        hr = CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice, out IInspectable inspectable);
        if (FAILED(hr))
        {
            direct3DDevice = default;
            return false;
        }

        direct3DDevice = inspectable.ObjRef.AsInterface<IDirect3DDevice>();
        return true;
    }

    public static unsafe bool TryCreateSwapChainForComposition(ObjectReference<IDXGIFactory6.Vftbl> factory, ObjectReference<ID3D11Device.Vftbl> device, ref readonly DXGI_SWAP_CHAIN_DESC1 desc, out ObjectReference<IDXGISwapChain1.Vftbl> swapChain, out HRESULT hr)
    {
        hr = ((IDXGIFactory6*)factory.ThisPtr)->CreateSwapChainForComposition(device, in desc, default, out swapChain);
        if (FAILED(hr))
        {
            return false;
        }

        return true;
    }
}