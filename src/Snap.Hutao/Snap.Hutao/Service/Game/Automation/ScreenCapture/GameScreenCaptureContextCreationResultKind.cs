// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal enum GameScreenCaptureContextCreationResultKind
{
    Success,
    CreateDxgiFactoryFailed,
    EnumAdapterByGpuPreferenceFailed,
    D3D11CreateDeviceFailed,
    D3D11DeviceQueryDXGIDeviceFailed,
    CreateDirect3D11DeviceFromDXGIDeviceFailed,
}