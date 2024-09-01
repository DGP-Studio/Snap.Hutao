// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal sealed partial class GameScreenCaptureResult : IDisposable
{
    private readonly IMemoryOwner<byte> rawPixelData;
    private readonly int pixelWidth;
    private readonly int pixelHeight;

    public GameScreenCaptureResult(IMemoryOwner<byte> rawPixelData, int pixelWidth, int pixelHeight)
    {
        this.rawPixelData = rawPixelData;
        this.pixelWidth = pixelWidth;
        this.pixelHeight = pixelHeight;
    }

    public int PixelWidth { get => pixelWidth; }

    public int PixelHeight { get => pixelHeight; }

    public void Dispose()
    {
        rawPixelData.Dispose();
    }
}