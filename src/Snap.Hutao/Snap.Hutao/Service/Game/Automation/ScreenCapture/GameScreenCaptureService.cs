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
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameScreenCaptureService))]
internal sealed partial class GameScreenCaptureService : IGameScreenCaptureService
{
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

        D3D11_CREATE_DEVICE_FLAG flag = D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT
#if DEBUG
            | D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG
#endif
            ;

        HRESULT hr;
        hr = D3D11CreateDevice(default, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, default, flag, [], D3D11_SDK_VERSION, out ID3D11Device* pD3D11Device, out _, out _);
        if (FAILED(hr))
        {
            logger.LogWarning("D3D11CreateDevice failed with code: {Code}", hr);
            return false;
        }

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