// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class D3d11
{
    [DllImport("d3d11.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern unsafe HRESULT CreateDirect3D11DeviceFromDXGIDevice(nint dxgiDevice, nint* graphicsDevice);

    [DebuggerStepThrough]
    public static unsafe HRESULT CreateDirect3D11DeviceFromDXGIDevice(ObjectReference<IDXGIDevice.Vftbl> dxgiDevice, out ObjectReference<IInspectable.Vftbl> graphicsDevice)
    {
        nint pGraphicsDevice = default;
        HRESULT hr = CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.ThisPtr, &pGraphicsDevice);
        graphicsDevice = ObjectReference<IInspectable.Vftbl>.Attach(ref pGraphicsDevice, IID.IID_IInspectable);
        return hr;
    }

    [DllImport("d3d11.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern unsafe HRESULT D3D11CreateDevice([Optional] nint pAdapter, D3D_DRIVER_TYPE DriverType, HMODULE Software, D3D11_CREATE_DEVICE_FLAG Flags, [Optional] D3D_FEATURE_LEVEL* pFeatureLevels, uint FeatureLevels, uint SDKVersion, [MaybeNull] nint* ppDevice, [MaybeNull] D3D_FEATURE_LEVEL* pFeatureLevel, [MaybeNull] nint* ppImmediateContext);

    public static unsafe HRESULT D3D11CreateDevice(ObjectReference<IDXGIAdapter.Vftbl>? adapter, D3D_DRIVER_TYPE DriverType, HMODULE Software, D3D11_CREATE_DEVICE_FLAG Flags, [Optional] ReadOnlySpan<D3D_FEATURE_LEVEL> featureLevels, uint SDKVersion, out ObjectReference<ID3D11Device.Vftbl>? device, out D3D_FEATURE_LEVEL featureLevel, out ObjectReference<ID3D11DeviceContext.Vftbl>? immediateContext)
    {
        fixed (D3D_FEATURE_LEVEL* pFeatureLevels = featureLevels)
        {
            fixed (D3D_FEATURE_LEVEL* pFeatureLevel = &featureLevel)
            {
                nint pDevice = default;
                nint pImmediateContext = default;
                HRESULT hr = D3D11CreateDevice(adapter?.ThisPtr ?? 0, DriverType, Software, Flags, pFeatureLevels, (uint)featureLevels.Length, SDKVersion, &pDevice, pFeatureLevel, &pImmediateContext);
                device = ObjectReference<ID3D11Device.Vftbl>.Attach(ref pDevice, ID3D11Device.IID);
                immediateContext = ObjectReference<ID3D11DeviceContext.Vftbl>.Attach(ref pImmediateContext, ID3D11DeviceContext.IID);
                return hr;
            }
        }
    }
}