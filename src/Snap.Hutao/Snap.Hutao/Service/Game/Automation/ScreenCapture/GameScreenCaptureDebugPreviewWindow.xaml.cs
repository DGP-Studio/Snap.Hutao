// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Win32.Graphics.Direct3D11;
using Snap.Hutao.Win32.Graphics.Dxgi;
using Snap.Hutao.Win32.System.WinRT.Graphics.Capture;
using Snap.Hutao.Win32.System.WinRT.Xaml;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal sealed partial class GameScreenCaptureDebugPreviewWindow : Window
{
    private unsafe IDXGISwapChain1* swapChain1;

    public GameScreenCaptureDebugPreviewWindow()
    {
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
        }

        InitializeComponent();
        this.InitializeController(Ioc.Default);
    }

    public unsafe void UpdateSwapChain(IDXGISwapChain1* swapChain1)
    {
        this.swapChain1 = swapChain1;
        ISwapChainPanelNative native = Presenter.As<IInspectable>().ObjRef.AsInterface<ISwapChainPanelNative>();
        native.SetSwapChain((IDXGISwapChain*)swapChain1);
    }

    public unsafe void UnsafeUpdatePreview(ID3D11Device* device, IDirect3DSurface surface)
    {
        IDirect3DDxgiInterfaceAccess access = surface.As<IDirect3DDxgiInterfaceAccess>();
        swapChain1->GetBuffer(0, in ID3D11Texture2D.IID, out ID3D11Texture2D* buffer);
        device->GetImmediateContext(out ID3D11DeviceContext* deviceContext);
        access.GetInterface(in ID3D11Resource.IID, out ID3D11Resource* resource);
        deviceContext->CopyResource((ID3D11Resource*)buffer, resource);
        swapChain1->Present(0, default);
    }
}
