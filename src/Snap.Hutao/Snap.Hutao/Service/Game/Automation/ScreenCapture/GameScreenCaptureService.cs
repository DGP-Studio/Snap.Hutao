// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.System.Com;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.D3d11;
using static Snap.Hutao.Win32.Dxgi;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameScreenCaptureService))]
internal sealed partial class GameScreenCaptureService : IGameScreenCaptureService
{
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

    private readonly ILogger<GameScreenCaptureService> logger;

    public bool IsSupported()
    {
        if (!Core.UniversalApiContract.IsPresent(WindowsVersion.Windows10Version1903))
        {
            logger.LogWarning("Windows 10 Version 1903 or later is required for Windows.Graphics.Capture API.");
            return false;
        }

        if (!GraphicsCaptureSession.IsSupported())
        {
            logger.LogWarning("GraphicsCaptureSession is not supported.");
            return false;
        }

        return true;
    }

    [SuppressMessage("", "SH002")]
    public unsafe bool TryStartCapture(HWND hwnd, [NotNullWhen(true)] out GameScreenCaptureSession? session)
    {
        session = default;

        HRESULT hr;
        hr = CreateDXGIFactory2(CreateDXGIFactoryFlag, in IDXGIFactory6.IID, out IDXGIFactory6* factory);
        if (FAILED(hr))
        {
            logger.LogWarning("CreateDXGIFactory2 failed with code: {Code}", hr);
            return false;
        }

        IUnknownMarshal.Release(factory);

        hr = factory->EnumAdapterByGpuPreference(0U, DXGI_GPU_PREFERENCE.DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, in IDXGIAdapter.IID, out IDXGIAdapter* adapter);
        if (hr != HRESULT.DXGI_ERROR_NOT_FOUND)
        {
            logger.LogWarning("IDXGIFactory6.EnumAdapterByGpuPreference failed with code: {Code}", hr);
            return false;
        }

        IUnknownMarshal.Release(adapter);

        hr = D3D11CreateDevice(adapter, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, default, D3d11CreateDeviceFlag, [], D3D11_SDK_VERSION, out ID3D11Device* pD3D11Device, out _, out _);
        if (FAILED(hr))
        {
            logger.LogWarning("D3D11CreateDevice failed with code: {Code}", hr);
            return false;
        }

        IUnknownMarshal.Release(pD3D11Device);

        hr = IUnknownMarshal.QueryInterface(pD3D11Device, in IDXGIDevice.IID, out IDXGIDevice* pDXGIDevice);
        if (FAILED(hr))
        {
            logger.LogWarning("ID3D11Device.QueryInterface<IDXGIDevice> failed with code: {Code}", hr);
            return false;
        }

        IUnknownMarshal.Release(pDXGIDevice);

        hr = CreateDirect3D11DeviceFromDXGIDevice(pDXGIDevice, out Win32.System.WinRT.IInspectable* inspectable);
        if (FAILED(hr))
        {
            logger.LogWarning("CreateDirect3D11DeviceFromDXGIDevice failed with code: {Code}", hr);
            return false;
        }

        IUnknownMarshal.Release(inspectable);

        IDirect3DDevice direct3DDevice = IInspectable.FromAbi((nint)inspectable).ObjRef.AsInterface<IDirect3DDevice>();

        GameScreenCaptureContext captureContext = new(direct3DDevice, hwnd);
        session = new(captureContext, logger);

        return true;
    }
}