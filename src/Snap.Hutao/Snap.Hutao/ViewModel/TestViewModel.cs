// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Hutao.HutaoAsAService;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.System.WinRT;
using Snap.Hutao.Win32.System.WinRT.Graphics.Capture;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
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
    private readonly MainWindow mainWindow;

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
        Must.NeverHappen();
    }

    [Command("ResetMainWindowSizeCommand")]
    private void ResetMainWindowSize()
    {
        double scale = mainWindow.WindowOptions.GetRasterizationScale();
        mainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1372, 772).Scale(scale));
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
        if (!UniversalApiContract.IsPresent(WindowsVersion.Windows10Version1903))
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

        ID3D11Device* pD3D11Device = default;
        if (SUCCEEDED(D3D11CreateDevice(default, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, default, flag, default, 0, D3D11_SDK_VERSION, &pD3D11Device, default, default)))
        {
            if (SUCCEEDED(IUnknownMarshal.QueryInterface(pD3D11Device, in IDXGIDevice.IID, out IDXGIDevice* pDXGIDevice)))
            {
                if (SUCCEEDED(CreateDirect3D11DeviceFromDXGIDevice(pDXGIDevice, out IInspectable* inspectable)))
                {
                    IDirect3DDevice direct3DDevice = WinRT.CastExtensions.As<IDirect3DDevice>(WinRT.IInspectable.FromAbi((nint)inspectable));

                    SizeInt32 size = new(1920, 1080);
                    Direct3D11CaptureFramePool framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(direct3DDevice, DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, size);
                    GC.KeepAlive(framePool);
                    framePool.FrameArrived += (pool, obj) =>
                    {
                        using (Direct3D11CaptureFrame frame = framePool.TryGetNextFrame())
                        {
                            if (frame is not null)
                            {
                                logger.LogInformation("Content Size: {Width} x {Height}", frame.ContentSize.Width, frame.ContentSize.Height);
                            }
                            else
                            {
                                logger.LogInformation("Null Frame");
                            }
                        }
                    };

                    HWND hwnd = serviceProvider.GetRequiredService<ICurrentWindowReference>().GetWindowHandle();

                    GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>().CreateForWindow(hwnd, out GraphicsCaptureItem item);

                    GraphicsCaptureSession captureSession = framePool.CreateCaptureSession(item);
                    captureSession.StartCapture();

                    Thread.Sleep(1000);
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

        _ = 1;
    }
}