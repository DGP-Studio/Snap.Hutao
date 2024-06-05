// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal readonly struct GameScreenCaptureContextCreationResult
{
    public readonly GameScreenCaptureContextCreationResultKind Kind;
    public readonly HRESULT HResult;
    public readonly GameScreenCaptureContext Context;

    public GameScreenCaptureContextCreationResult(GameScreenCaptureContextCreationResultKind kind, HRESULT hResult)
    {
        Kind = kind;
        HResult = hResult;
    }

    public GameScreenCaptureContextCreationResult(GameScreenCaptureContextCreationResultKind kind, GameScreenCaptureContext context)
    {
        Kind = kind;
        Context = context;
    }
}