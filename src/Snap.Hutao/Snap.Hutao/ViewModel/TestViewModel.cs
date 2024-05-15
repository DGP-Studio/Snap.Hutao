// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Hutao.HutaoAsAService;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.System.WinRT;
using Snap.Hutao.Win32.System.WinRT.Graphics.Capture;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using WinRT;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.D3D11;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 测试视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class TestViewModel : Abstraction.ViewModel
{
    private readonly HutaoAsAServiceClient homaAsAServiceClient;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ILogger<TestViewModel> logger;
    private readonly IMemoryCache memoryCache;
    private readonly ITaskContext taskContext;

    private UploadAnnouncement announcement = new();

    public UploadAnnouncement Announcement { get => announcement; set => SetProperty(ref announcement, value); }

    public bool SuppressMetadataInitialization
    {
        get => LocalSetting.Get(SettingKeys.SuppressMetadataInitialization, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.SuppressMetadataInitialization, value);
        }
    }

    public bool OverrideElevationRequirement
    {
        get => LocalSetting.Get(SettingKeys.OverrideElevationRequirement, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.OverrideElevationRequirement, value);
        }
    }

    public bool OverrideUpdateVersionComparison
    {
        get => LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.OverrideUpdateVersionComparison, value);
        }
    }

    public bool OverridePackageConvertDirectoryPermissionsRequirement
    {
        get => LocalSetting.Get(SettingKeys.OverridePackageConvertDirectoryPermissionsRequirement, false);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            LocalSetting.Set(SettingKeys.OverridePackageConvertDirectoryPermissionsRequirement, value);
        }
    }

    [Command("ResetGuideStateCommand")]
    private static void ResetGuideState()
    {
        UnsafeLocalSetting.Set(SettingKeys.Major1Minor10Revision0GuideState, GuideState.Language);
    }

    [Command("ExceptionCommand")]
    private static void ThrowTestException()
    {
        HutaoException.Throw("Test Exception");
    }

    [Command("ResetMainWindowSizeCommand")]
    private void ResetMainWindowSize()
    {
        if (serviceProvider.GetRequiredService<ICurrentXamlWindowReference>().Window is MainWindow mainWindow)
        {
            double scale = mainWindow.GetRasterizationScale();
            mainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1372, 772).Scale(scale));
        }
    }

    [Command("UploadAnnouncementCommand")]
    private async Task UploadAnnouncementAsync()
    {
        Web.Response.Response response = await homaAsAServiceClient.UploadAnnouncementAsync(Announcement).ConfigureAwait(false);
        if (response.IsOk())
        {
            infoBarService.Success(response.Message);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = new();
        }
    }

    [Command("CompensationGachaLogServiceTimeCommand")]
    private async Task CompensationGachaLogServiceTimeAsync()
    {
        Web.Response.Response response = await homaAsAServiceClient.GachaLogCompensationAsync(15).ConfigureAwait(false);
        if (response.IsOk())
        {
            infoBarService.Success(response.Message);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = new();
        }
    }

    [Command("DebugPrintImageCacheFailedDownloadTasksCommand")]
    private void DebugPrintImageCacheFailedDownloadTasks()
    {
        if (memoryCache.TryGetValue($"{nameof(ImageCache)}.FailedDownloadTasks", out HashSet<string>? set))
        {
            logger.LogInformation("Failed ImageCache download tasks: [{Tasks}]", set?.ToString(','));
        }
    }

    [Command("TestWindowsGraphicsCaptureCommand")]
    private unsafe void TestWindowsGraphicsCapture()
    {
        // https://github.com/obsproject/obs-studio/blob/master/libobs-winrt/winrt-capture.cpp
        if (!Core.UniversalApiContract.IsPresent(WindowsVersion.Windows10Version1903))
        {
            logger.LogWarning("Windows 10 Version 1903 or later is required for Windows.Graphics.Capture API.");
            return;
        }

        if (!GraphicsCaptureSession.IsSupported())
        {
            logger.LogWarning("GraphicsCaptureSession is not supported.");
            return;
        }

        D3D11_CREATE_DEVICE_FLAG flag = D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT | D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;

        if (SUCCEEDED(D3D11CreateDevice(default, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, default, flag, [], D3D11_SDK_VERSION, out ID3D11Device* pD3D11Device, out _, out _)))
        {
            if (SUCCEEDED(IUnknownMarshal.QueryInterface(pD3D11Device, in IDXGIDevice.IID, out IDXGIDevice* pDXGIDevice)))
            {
                if (SUCCEEDED(CreateDirect3D11DeviceFromDXGIDevice(pDXGIDevice, out Win32.System.WinRT.IInspectable* inspectable)))
                {
                    IDirect3DDevice direct3DDevice = WinRT.IInspectable.FromAbi((nint)inspectable).ObjRef.AsInterface<IDirect3DDevice>();

                    HWND hwnd = serviceProvider.GetRequiredService<ICurrentXamlWindowReference>().GetWindowHandle();
                    GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>().CreateForWindow(hwnd, out GraphicsCaptureItem item);

                    using (Direct3D11CaptureFramePool framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(direct3DDevice, DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, item.Size))
                    {
                        framePool.FrameArrived += (pool, _) =>
                        {
                            using (Direct3D11CaptureFrame frame = pool.TryGetNextFrame())
                            {
                                if (frame is not null)
                                {
                                    logger.LogInformation("Content Size: {Width} x {Height}", frame.ContentSize.Width, frame.ContentSize.Height);

                                    IDirect3DDxgiInterfaceAccess access = frame.Surface.As<IDirect3DDxgiInterfaceAccess>();
                                    if (FAILED(access.GetInterface(in IDXGISurface.IID, out IDXGISurface* pDXGISurface)))
                                    {
                                        return;
                                    }

                                    if (FAILED(pDXGISurface->GetDesc(out DXGI_SURFACE_DESC surfaceDesc)))
                                    {
                                        return;
                                    }

                                    D3D11_TEXTURE2D_DESC texture2DDesc = default;
                                    texture2DDesc.Width = surfaceDesc.Width;
                                    texture2DDesc.Height = surfaceDesc.Height;
                                    texture2DDesc.ArraySize = 1;
                                    texture2DDesc.CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_READ;
                                    texture2DDesc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
                                    texture2DDesc.MipLevels = 1;
                                    texture2DDesc.SampleDesc.Count = 1;
                                    texture2DDesc.Usage = D3D11_USAGE.D3D11_USAGE_STAGING;

                                    if (FAILED(pDXGISurface->GetDevice(in ID3D11Device.IID, out ID3D11Device* pD3D11Device)))
                                    {
                                        return;
                                    }

                                    if (FAILED(pD3D11Device->CreateTexture2D(ref texture2DDesc, ref Unsafe.NullRef<D3D11_SUBRESOURCE_DATA>(), out ID3D11Texture2D* pTexture2D)))
                                    {
                                        return;
                                    }

                                    if (FAILED(access.GetInterface(in ID3D11Resource.IID, out ID3D11Resource* pD3D11Resource)))
                                    {
                                        return;
                                    }

                                    pD3D11Device->GetImmediateContext(out ID3D11DeviceContext* pDeviceContext);
                                    pDeviceContext->CopyResource((ID3D11Resource*)pTexture2D, pD3D11Resource);

                                    if (FAILED(pDeviceContext->Map((ID3D11Resource*)pTexture2D, 0, D3D11_MAP.D3D11_MAP_READ, 0, out D3D11_MAPPED_SUBRESOURCE mappedSubresource)))
                                    {
                                        return;
                                    }

                                    int size = (int)(mappedSubresource.RowPitch * texture2DDesc.Height * 4);

                                    SoftwareBitmap softwareBitmap = new(BitmapPixelFormat.Bgra8, (int)texture2DDesc.Width, (int)texture2DDesc.Height, BitmapAlphaMode.Premultiplied);
                                    using (BitmapBuffer bitmapBuffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Write))
                                    {
                                        using (IMemoryBufferReference reference = bitmapBuffer.CreateReference())
                                        {
                                            reference.As<IMemoryBufferByteAccess>().GetBuffer(out Span<byte> bufferSpan);
                                            fixed (byte* p = bufferSpan)
                                            {
                                                for (uint i = 0; i < texture2DDesc.Height; i++)
                                                {
                                                    System.Buffer.MemoryCopy(((byte*)mappedSubresource.pData) + (i * mappedSubresource.RowPitch), p + (i * texture2DDesc.Width * 4), texture2DDesc.Width * 4, texture2DDesc.Width * 4);
                                                }
                                            }
                                        }
                                    }

                                    using (InMemoryRandomAccessStream stream = new())
                                    {
                                        BitmapEncoder encoder = BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream).AsTask().Result;
                                        encoder.SetSoftwareBitmap(softwareBitmap);
                                        encoder.FlushAsync().AsTask().Wait();

                                        using (FileStream fileStream = new("D:\\test.png", FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                                        {
                                            stream.AsStream().CopyTo(fileStream);
                                        }
                                    }

                                    _ = 1;

                                    pDeviceContext->Unmap((ID3D11Resource*)pTexture2D, 0);
                                }
                                else
                                {
                                    logger.LogInformation("Null Frame");
                                }
                            }
                        };

                        using (GraphicsCaptureSession captureSession = framePool.CreateCaptureSession(item))
                        {
                            captureSession.IsCursorCaptureEnabled = false;
                            captureSession.IsBorderRequired = false;
                            captureSession.StartCapture();

                            Thread.Sleep(1000);
                        }
                    }
                }
                else
                {
                    logger.LogWarning("CreateDirect3D11DeviceFromDXGIDevice failed");
                }

                IUnknownMarshal.Release(pDXGIDevice);
            }
            else
            {
                logger.LogWarning("ID3D11Device As IDXGIDevice failed");
            }
        }
        else
        {
            logger.LogWarning("D3D11CreateDevice failed");
        }
    }
}