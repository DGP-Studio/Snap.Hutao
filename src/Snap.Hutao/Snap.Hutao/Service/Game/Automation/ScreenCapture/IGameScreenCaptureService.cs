// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal interface IGameScreenCaptureService
{
    bool IsSupported();

    bool TryStartCapture(HWND hwnd, bool preview, [NotNullWhen(true)] out GameScreenCaptureSession? session);
}