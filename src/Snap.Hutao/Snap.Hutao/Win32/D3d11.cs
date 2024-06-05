// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.System.WinRT;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class D3d11
{
    [DllImport("d3d11.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static unsafe extern HRESULT CreateDirect3D11DeviceFromDXGIDevice(IDXGIDevice* dxgiDevice, IInspectable** graphicsDevice);

    [DebuggerStepThrough]
    public static unsafe HRESULT CreateDirect3D11DeviceFromDXGIDevice(IDXGIDevice* dxgiDevice, out IInspectable* graphicsDevice)
    {
        fixed (IInspectable** pGraphicsDevice = &graphicsDevice)
        {
            return CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice, pGraphicsDevice);
        }
    }

    [DllImport("d3d11.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static unsafe extern HRESULT D3D11CreateDevice([AllowNull] IDXGIAdapter* pAdapter, D3D_DRIVER_TYPE DriverType, HMODULE Software, D3D11_CREATE_DEVICE_FLAG Flags, [AllowNull] D3D_FEATURE_LEVEL* pFeatureLevels, uint FeatureLevels, uint SDKVersion, [MaybeNull] ID3D11Device** ppDevice, [MaybeNull] D3D_FEATURE_LEVEL* pFeatureLevel, [MaybeNull] ID3D11DeviceContext** ppImmediateContext);

    [SuppressMessage("", "SH002")]
    public static unsafe HRESULT D3D11CreateDevice([AllowNull] IDXGIAdapter* pAdapter, D3D_DRIVER_TYPE DriverType, HMODULE Software, D3D11_CREATE_DEVICE_FLAG Flags, [AllowNull] ReadOnlySpan<D3D_FEATURE_LEVEL> featureLevels, uint SDKVersion, out ID3D11Device* pDevice, out D3D_FEATURE_LEVEL featureLevel, out ID3D11DeviceContext* pImmediateContext)
    {
        fixed (ID3D11Device** ppDevice = &pDevice)
        {
            fixed (D3D_FEATURE_LEVEL* pFeatureLevels = featureLevels)
            {
                fixed (D3D_FEATURE_LEVEL* pFeatureLevel = &featureLevel)
                {
                    fixed (ID3D11DeviceContext** ppImmediateContext = &pImmediateContext)
                    {
                        return D3D11CreateDevice(pAdapter, DriverType, Software, Flags, pFeatureLevels, (uint)featureLevels.Length, SDKVersion, ppDevice, pFeatureLevel, ppImmediateContext);
                    }
                }
            }
        }
    }
}