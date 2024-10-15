// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Win32.Foundation;
using Windows.Graphics.Capture;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameScreenCaptureService))]
internal sealed partial class GameScreenCaptureService : IGameScreenCaptureService
{
    private readonly ILogger<GameScreenCaptureService> logger;

    public bool IsSupported()
    {
        if (!UniversalApiContract.IsPresent(WindowsVersion.Windows10Version1903))
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

    public bool TryStartCapture(HWND hwnd, bool preview, [NotNullWhen(true)] out GameScreenCaptureSession? session)
    {
        session = default;

        GameScreenCaptureContextCreationResult result = GameScreenCaptureContext.Create(hwnd, preview);

        switch (result.Kind)
        {
            case GameScreenCaptureContextCreationResultKind.Success:
                session = new(result.Context, logger);
                return true;
            case GameScreenCaptureContextCreationResultKind.CreateDxgiFactoryFailed:
                logger.LogWarning("CreateDXGIFactory2 failed with code: {Code}", result.HResult);
                return false;
            case GameScreenCaptureContextCreationResultKind.EnumAdapterByGpuPreferenceFailed:
                logger.LogWarning("IDXGIFactory6.EnumAdapterByGpuPreference failed with code: {Code}", result.HResult);
                return false;
            case GameScreenCaptureContextCreationResultKind.D3D11CreateDeviceFailed:
                logger.LogWarning("D3D11CreateDevice failed with code: {Code}", result.HResult);
                return false;
            case GameScreenCaptureContextCreationResultKind.D3D11DeviceQueryDXGIDeviceFailed:
                logger.LogWarning("ID3D11Device.QueryInterface<IDXGIDevice> failed with code: {Code}", result.HResult);
                return false;
            case GameScreenCaptureContextCreationResultKind.CreateDirect3D11DeviceFromDXGIDeviceFailed:
                logger.LogWarning("CreateDirect3D11DeviceFromDXGIDevice failed with code: {Code}", result.HResult);
                return false;
            default:
                return false;
        }
    }
}