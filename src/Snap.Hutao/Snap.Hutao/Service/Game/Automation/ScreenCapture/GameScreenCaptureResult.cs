// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal sealed partial class GameScreenCaptureResult : IDisposable
{
    private readonly IMemoryOwner<byte> rawPixelData;

    public GameScreenCaptureResult(IMemoryOwner<byte> rawPixelData, int pixelWidth, int pixelHeight)
    {
        this.rawPixelData = rawPixelData;
        PixelWidth = pixelWidth;
        PixelHeight = pixelHeight;
    }

    public int PixelWidth { get; }

    public int PixelHeight { get; }

    public void Dispose()
    {
        rawPixelData.Dispose();
    }
}