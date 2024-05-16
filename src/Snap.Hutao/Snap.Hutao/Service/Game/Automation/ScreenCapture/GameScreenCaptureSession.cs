// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using Snap.Hutao.Win32.System.WinRT.Graphics.Capture;
using System.Buffers;
using System.Runtime.CompilerServices;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal sealed class GameScreenCaptureSession : IDisposable
{
    private readonly GameScreenCaptureContext captureContext;
    private readonly Direct3D11CaptureFramePool framePool;
    private readonly GraphicsCaptureSession session;
    private readonly ILogger logger;

    private TaskCompletionSource<IMemoryOwner<byte>>? frameRawPixelDataTaskCompletionSource;
    private bool isFrameRawPixelDataRequested;
    private SizeInt32 contentSize;

    private bool isDisposed;

    [SuppressMessage("", "SH002")]
    public GameScreenCaptureSession(GameScreenCaptureContext captureContext, ILogger logger)
    {
        this.captureContext = captureContext;
        this.logger = logger;

        contentSize = captureContext.Item.Size;

        captureContext.Item.Closed += OnItemClosed;

        framePool = captureContext.CreatePool();
        framePool.FrameArrived += OnFrameArrived;

        session = captureContext.CreateSession(framePool);
        session.StartCapture();
    }

    public async ValueTask<IMemoryOwner<byte>> RequestFrameRawPixelDataAsync()
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
                UnsafeProcessFrameSurface(frame.Surface);
            }
            catch (Exception ex) // TODO: test if it's device lost.
            {
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
        if (FAILED(access.GetInterface(in IDXGISurface.IID, out IDXGISurface* pDXGISurface)))
        {
            return;
        }

        if (FAILED(pDXGISurface->GetDesc(out DXGI_SURFACE_DESC dxgiSurfaceDesc)))
        {
            return;
        }

        bool boxAvailable = captureContext.TryGetClientBox(dxgiSurfaceDesc.Width, dxgiSurfaceDesc.Height, out D3D11_BOX clientBox);
        (uint textureWidth, uint textureHeight) = boxAvailable
            ? (clientBox.right - clientBox.left, clientBox.bottom - clientBox.top)
            : (dxgiSurfaceDesc.Width, dxgiSurfaceDesc.Height);

        // Should be the same device used to create the frame pool.
        if (FAILED(pDXGISurface->GetDevice(in ID3D11Device.IID, out ID3D11Device* pD3D11Device)))
        {
            return;
        }

        D3D11_TEXTURE2D_DESC d3d11Texture2DDesc = default;
        d3d11Texture2DDesc.Width = textureWidth;
        d3d11Texture2DDesc.Height = textureHeight;
        d3d11Texture2DDesc.Format = dxgiSurfaceDesc.Format;
        d3d11Texture2DDesc.CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_READ;
        d3d11Texture2DDesc.Usage = D3D11_USAGE.D3D11_USAGE_STAGING;
        d3d11Texture2DDesc.SampleDesc.Count = 1;
        d3d11Texture2DDesc.ArraySize = 1;
        d3d11Texture2DDesc.MipLevels = 1;

        if (FAILED(pD3D11Device->CreateTexture2D(ref d3d11Texture2DDesc, ref Unsafe.NullRef<D3D11_SUBRESOURCE_DATA>(), out ID3D11Texture2D* pD3D11Texture2D)))
        {
            return;
        }

        if (FAILED(access.GetInterface(in ID3D11Resource.IID, out ID3D11Resource* pD3D11Resource)))
        {
            return;
        }

        pD3D11Device->GetImmediateContext(out ID3D11DeviceContext* pD3D11DeviceContext);

        if (boxAvailable)
        {

        }
        else
        {
            pD3D11DeviceContext->CopyResource((ID3D11Resource*)pD3D11Texture2D, pD3D11Resource);
        }

        if (FAILED(pD3D11DeviceContext->Map((ID3D11Resource*)pD3D11Texture2D, 0U, D3D11_MAP.D3D11_MAP_READ, 0U, out D3D11_MAPPED_SUBRESOURCE d3d11MappedSubresource)))
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
        ReadOnlySpan2D<byte> subresource = new(d3d11MappedSubresource.pData, (int)d3d11Texture2DDesc.Height, (int)d3d11MappedSubresource.RowPitch);

        int rowLength = contentSize.Width * 4;
        IMemoryOwner<byte> buffer = GameScreenCaptureMemoryPool.Shared.Rent(contentSize.Height * rowLength);

        for (int row = 0; row < contentSize.Height; row++)
        {
            subresource[row][..rowLength].CopyTo(buffer.Memory.Span.Slice(row * rowLength, rowLength));
        }

        ArgumentNullException.ThrowIfNull(frameRawPixelDataTaskCompletionSource);
        frameRawPixelDataTaskCompletionSource.SetResult(buffer);
    }
}