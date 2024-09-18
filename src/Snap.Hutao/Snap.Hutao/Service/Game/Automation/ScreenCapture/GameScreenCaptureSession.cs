﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.UI;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using Snap.Hutao.Win32.System.WinRT.Graphics.Capture;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal sealed partial class GameScreenCaptureSession : IDisposable
{
    private static readonly Half ByteMaxValue = 255;

    private readonly GameScreenCaptureContext captureContext;
    private readonly GameScreenCaptureDebugPreviewWindow? previewWindow;
    private readonly Direct3D11CaptureFramePool framePool;
    private readonly GraphicsCaptureSession session;
    private readonly ILogger logger;

    private TaskCompletionSource<GameScreenCaptureResult>? frameRawPixelDataTaskCompletionSource;
    private bool isFrameRawPixelDataRequested;
    private SizeInt32 contentSize;

    private bool isDisposed;

    public unsafe GameScreenCaptureSession(GameScreenCaptureContext captureContext, ILogger logger)
    {
        this.captureContext = captureContext;
        this.logger = logger;

        contentSize = captureContext.Item.Size;

        if (captureContext.PreviewEnabled)
        {
            previewWindow = new();
        }

        captureContext.Item.Closed += OnItemClosed;

        framePool = captureContext.CreatePool();
        captureContext.AttachPreview(previewWindow);

        framePool.FrameArrived += OnFrameArrived;

        session = captureContext.CreateSession(framePool);
        session.StartCapture();
    }

    public async ValueTask<GameScreenCaptureResult> RequestFrameAsync()
    {
        if (Volatile.Read(ref isFrameRawPixelDataRequested))
        {
            HutaoException.InvalidOperation("The frame raw pixel data has already been requested.");
        }

        if (isDisposed)
        {
            HutaoException.InvalidOperation("The session has been disposed.");
        }

        frameRawPixelDataTaskCompletionSource = new();
        Volatile.Write(ref isFrameRawPixelDataRequested, true);

        return await frameRawPixelDataTaskCompletionSource.Task.ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        captureContext.DetachPreview(previewWindow);
        session.Dispose();
        framePool.Dispose();
        isDisposed = true;
    }

    private void OnItemClosed(GraphicsCaptureItem sender, object args)
    {
        Dispose();
    }

    private unsafe void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
    {
        // Simply ignore the frame if the frame raw pixel data is not requested.
        if (!Volatile.Read(ref isFrameRawPixelDataRequested))
        {
            return;
        }

        using (Direct3D11CaptureFrame? frame = sender.TryGetNextFrame())
        {
            if (frame is null)
            {
                return;
            }

            bool needsReset = false;

            if (frame.ContentSize != contentSize)
            {
                needsReset = true;
                contentSize = frame.ContentSize;
            }

            try
            {
                captureContext.UpdatePreview(previewWindow, frame.Surface);

                // UnsafeProcessFrameSurface(frame.Surface);
            }
            catch (Exception ex)
            {
                // TODO: test if it's device lost.
                logger.LogError(ex, "Failed to process the frame surface.");
                needsReset = true;
            }

            if (needsReset)
            {
                captureContext.RecreatePool(sender);
            }
        }
    }

    private unsafe void UnsafeProcessFrameSurface(IDirect3DSurface surface)
    {
        IDirect3DDxgiInterfaceAccess access = surface.As<IDirect3DDxgiInterfaceAccess>();
        if (FAILED(access.GetInterface(in IDXGISurface.IID, out ObjectReference<IDXGISurface.Vftbl> dxgiSurface)))
        {
            return;
        }

        using (dxgiSurface)
        {
            IDXGISurface* pDXGISurface = (IDXGISurface*)dxgiSurface.ThisPtr;
            if (FAILED(pDXGISurface->GetDesc(out DXGI_SURFACE_DESC dxgiSurfaceDesc)))
            {
                return;
            }

            bool boxAvailable = captureContext.TryGetClientBox(dxgiSurfaceDesc.Width, dxgiSurfaceDesc.Height, out D3D11_BOX clientBox);
            (uint textureWidth, uint textureHeight) = boxAvailable
                ? (clientBox.right - clientBox.left, clientBox.bottom - clientBox.top)
                : (dxgiSurfaceDesc.Width, dxgiSurfaceDesc.Height);

            // Should be the same device used to create the frame pool.
            if (FAILED(pDXGISurface->GetDevice(in ID3D11Device.IID, out ObjectReference<ID3D11Device.Vftbl> d3d11Device)))
            {
                return;
            }

            using (d3d11Device)
            {
                ID3D11Device* pD3D11Device = (ID3D11Device*)d3d11Device.ThisPtr;

                D3D11_TEXTURE2D_DESC d3d11Texture2DDesc = default;
                d3d11Texture2DDesc.Width = textureWidth;
                d3d11Texture2DDesc.Height = textureHeight;
                d3d11Texture2DDesc.Format = dxgiSurfaceDesc.Format;
                d3d11Texture2DDesc.CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_READ;
                d3d11Texture2DDesc.Usage = D3D11_USAGE.D3D11_USAGE_STAGING;
                d3d11Texture2DDesc.SampleDesc.Count = 1;
                d3d11Texture2DDesc.ArraySize = 1;
                d3d11Texture2DDesc.MipLevels = 1;

                if (FAILED(pD3D11Device->CreateTexture2D(ref d3d11Texture2DDesc, ref Unsafe.NullRef<D3D11_SUBRESOURCE_DATA>(), out ObjectReference<ID3D11Texture2D.Vftbl>? d3d11Texture2D)))
                {
                    return;
                }

                using (d3d11Texture2D)
                {
                    ID3D11Texture2D* pD3D11Texture2D = (ID3D11Texture2D*)d3d11Texture2D.ThisPtr;

                    if (FAILED(access.GetInterface(in ID3D11Resource.IID, out ObjectReference<ID3D11Resource.Vftbl> d3d11Resource)))
                    {
                        return;
                    }

                    using (d3d11Resource)
                    {
                        ID3D11Resource* pD3D11Resource = (ID3D11Resource*)d3d11Resource.ThisPtr;

                        pD3D11Device->GetImmediateContext(out ObjectReference<ID3D11DeviceContext.Vftbl> d3d11DeviceContext);

                        using (d3d11DeviceContext)
                        {
                            ID3D11DeviceContext* pD3D11DeviceContext = (ID3D11DeviceContext*)d3d11DeviceContext.ThisPtr;

                            using (ObjectReference<ID3D11Resource.Vftbl> d3d11Texture2DAsResource = d3d11Texture2D.As<ID3D11Resource.Vftbl>(ID3D11Resource.IID))
                            {
                                if (boxAvailable)
                                {
                                    pD3D11DeviceContext->CopySubresourceRegion(d3d11Texture2DAsResource, 0U, 0U, 0U, 0U, d3d11Resource, 0U, in clientBox);
                                }
                                else
                                {
                                    pD3D11DeviceContext->CopyResource(d3d11Texture2DAsResource, d3d11Resource);
                                }

                                if (FAILED(pD3D11DeviceContext->Map(d3d11Texture2DAsResource, 0U, D3D11_MAP.D3D11_MAP_READ, 0U, out D3D11_MAPPED_SUBRESOURCE d3d11MappedSubresource)))
                                {
                                    return;
                                }

                                // The D3D11_MAPPED_SUBRESOURCE data is arranged as follows:
                                // |--------- Row pitch ----------|
                                // |---- Data width ----|- Blank -|
                                // ┌────────────────────┬─────────┐
                                // │                    │         │
                                // │     Actual data    │ Stride  │
                                // │                    │         │
                                // └────────────────────┴─────────┘
                                ReadOnlySpan2D<byte> subresource = new(d3d11MappedSubresource.pData, (int)d3d11MappedSubresource.RowPitch);

                                ArgumentNullException.ThrowIfNull(frameRawPixelDataTaskCompletionSource);
                                switch (dxgiSurfaceDesc.Format)
                                {
                                    case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM:
                                        {
                                            int rowLength = (int)textureWidth * 4;
                                            IMemoryOwner<byte> buffer = GameScreenCaptureMemoryPool.Shared.Rent((int)(textureHeight * textureWidth * 4));

                                            for (int row = 0; row < textureHeight; row++)
                                            {
                                                subresource[row][..rowLength].CopyTo(buffer.Memory.Span.Slice(row * rowLength, rowLength));
                                            }
#pragma warning disable CA2000
                                            frameRawPixelDataTaskCompletionSource.SetResult(new(buffer, (int)textureWidth, (int)textureHeight));
#pragma warning restore CA2000
                                            return;
                                        }

                                    case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT:
                                        {
                                            // TODO: replace with HLSL implementation.
                                            int rowLength = (int)textureWidth * 8;
                                            IMemoryOwner<byte> buffer = GameScreenCaptureMemoryPool.Shared.Rent((int)(textureHeight * textureWidth * 4));
                                            Span<Bgra32> pixelBuffer = MemoryMarshal.Cast<byte, Bgra32>(buffer.Memory.Span);

                                            for (int row = 0; row < textureHeight; row++)
                                            {
                                                ReadOnlySpan<Rgba64> subresourceRow = MemoryMarshal.Cast<byte, Rgba64>(subresource[row][..rowLength]);
                                                Span<Bgra32> bufferRow = pixelBuffer.Slice(row * (int)textureWidth, (int)textureWidth);
                                                for (int column = 0; column < textureWidth; column++)
                                                {
                                                    ref readonly Rgba64 float16Pixel = ref subresourceRow[column];
                                                    ref Bgra32 pixel = ref bufferRow[column];
                                                    pixel.B = (byte)(float16Pixel.B * ByteMaxValue);
                                                    pixel.G = (byte)(float16Pixel.G * ByteMaxValue);
                                                    pixel.R = (byte)(float16Pixel.R * ByteMaxValue);
                                                    pixel.A = (byte)(float16Pixel.A * ByteMaxValue);
                                                }
                                            }
#pragma warning disable CA2000
                                            frameRawPixelDataTaskCompletionSource.SetResult(new(buffer, (int)textureWidth, (int)textureHeight));
#pragma warning restore CA2000
                                            return;
                                        }

                                    default:
                                        HutaoException.NotSupported($"Unexpected DXGI_FORMAT: {dxgiSurfaceDesc.Format}");
                                        return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}