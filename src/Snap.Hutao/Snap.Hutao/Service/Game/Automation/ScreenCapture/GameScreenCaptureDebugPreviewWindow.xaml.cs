// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.System.WinRT.Graphics.Capture;
using Snap.Hutao.Win32.System.WinRT.Xaml;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal sealed partial class GameScreenCaptureDebugPreviewWindow : Window
{
    private ObjectReference<IDXGISwapChain1.Vftbl>? swapChain1;

    public GameScreenCaptureDebugPreviewWindow()
    {
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
        }

        InitializeComponent();
        this.InitializeController(Ioc.Default);
    }

    public unsafe void UpdateSwapChain(ObjectReference<IDXGISwapChain1.Vftbl>? swapChain1)
    {
        this.swapChain1 = swapChain1;
        ISwapChainPanelNative native = Presenter.As<IInspectable>().ObjRef.AsInterface<ISwapChainPanelNative>();
        native.SetSwapChain(swapChain1?.As<IDXGISwapChain.Vftbl>(IDXGISwapChain.IID));
    }

    public unsafe void UnsafeUpdatePreview(ObjectReference<ID3D11Device.Vftbl> device, IDirect3DSurface surface)
    {
        ArgumentNullException.ThrowIfNull(swapChain1);

        IDirect3DDxgiInterfaceAccess access = surface.As<IDirect3DDxgiInterfaceAccess>();
        swapChain1.GetBuffer(0, in ID3D11Texture2D.IID, out ObjectReference<ID3D11Texture2D.Vftbl> buffer);
        using (buffer)
        {
            device.GetImmediateContext(out ObjectReference<ID3D11DeviceContext.Vftbl> deviceContext);
            using (deviceContext)
            {
                access.GetInterface(in ID3D11Resource.IID, out ObjectReference<ID3D11Resource.Vftbl> resource);
                using (resource)
                {
                    deviceContext.CopyResource(buffer.As<ID3D11Resource.Vftbl>(ID3D11Resource.IID), resource);
                    swapChain1.Present(0, default);
                }
            }
        }
    }
}
