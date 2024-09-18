// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class D3d11
{
    [DllImport("d3d11.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern unsafe HRESULT CreateDirect3D11DeviceFromDXGIDevice(IDXGIDevice* dxgiDevice, System.WinRT.IInspectable** graphicsDevice);

    [DebuggerStepThrough]
    public static unsafe HRESULT CreateDirect3D11DeviceFromDXGIDevice(ObjectReference<IDXGIDevice.Vftbl> dxgiDevice, out IInspectable graphicsDevice)
    {
        System.WinRT.IInspectable* pGraphicsDevice = default;
        HRESULT hr = CreateDirect3D11DeviceFromDXGIDevice((IDXGIDevice*)dxgiDevice.ThisPtr, &pGraphicsDevice);
        graphicsDevice = new(ObjectReference<IUnknownVftbl>.Attach(ref Unsafe.AsRef<nint>(&pGraphicsDevice), IID.IID_IInspectable));
        return hr;
    }

    [DllImport("d3d11.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern unsafe HRESULT D3D11CreateDevice([AllowNull] IDXGIAdapter* pAdapter, D3D_DRIVER_TYPE DriverType, HMODULE Software, D3D11_CREATE_DEVICE_FLAG Flags, [AllowNull] D3D_FEATURE_LEVEL* pFeatureLevels, uint FeatureLevels, uint SDKVersion, [MaybeNull] ID3D11Device** ppDevice, [MaybeNull] D3D_FEATURE_LEVEL* pFeatureLevel, [MaybeNull] ID3D11DeviceContext** ppImmediateContext);

    public static unsafe HRESULT D3D11CreateDevice([AllowNull] ObjectReference<IDXGIAdapter.Vftbl> adapter, D3D_DRIVER_TYPE DriverType, HMODULE Software, D3D11_CREATE_DEVICE_FLAG Flags, [AllowNull] ReadOnlySpan<D3D_FEATURE_LEVEL> featureLevels, uint SDKVersion, out ObjectReference<ID3D11Device.Vftbl> device, out D3D_FEATURE_LEVEL featureLevel, out ObjectReference<ID3D11DeviceContext.Vftbl> immediateContext)
    {
        fixed (D3D_FEATURE_LEVEL* pFeatureLevels = featureLevels)
        {
            fixed (D3D_FEATURE_LEVEL* pFeatureLevel = &featureLevel)
            {
                ID3D11Device* pDevice = default;
                ID3D11DeviceContext* pImmediateContext = default;
                HRESULT hr = D3D11CreateDevice((IDXGIAdapter*)(adapter?.ThisPtr ?? 0), DriverType, Software, Flags, pFeatureLevels, (uint)featureLevels.Length, SDKVersion, &pDevice, pFeatureLevel, &pImmediateContext);
                device = ObjectReference<ID3D11Device.Vftbl>.Attach(ref Unsafe.AsRef<nint>(&pDevice), ID3D11Device.IID);
                immediateContext = ObjectReference<ID3D11DeviceContext.Vftbl>.Attach(ref Unsafe.AsRef<nint>(&pImmediateContext), ID3D11DeviceContext.IID);
                return hr;
            }
        }
    }
}