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
    private unsafe ObjectReference<IDXGISwapChain1.Vftbl>? swapChain1 = default;

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
        native.SetSwapChain((IDXGISwapChain*)(swapChain1?.ThisPtr ?? 0));
    }

    public unsafe void UnsafeUpdatePreview(ObjectReference<ID3D11Device.Vftbl> device, IDirect3DSurface surface)
    {
        ArgumentNullException.ThrowIfNull(swapChain1);

        IDirect3DDxgiInterfaceAccess access = surface.As<IDirect3DDxgiInterfaceAccess>();
        ((IDXGISwapChain1*)swapChain1.ThisPtr)->GetBuffer(0, in ID3D11Texture2D.IID, out ObjectReference<ID3D11Texture2D.Vftbl> buffer);
        using (buffer)
        {
            ((ID3D11Device*)device.ThisPtr)->GetImmediateContext(out ObjectReference<ID3D11DeviceContext.Vftbl> deviceContext);
            using (deviceContext)
            {
                access.GetInterface(in ID3D11Resource.IID, out ObjectReference<ID3D11Resource.Vftbl> resource);
                using (resource)
                {
                    ((ID3D11DeviceContext*)deviceContext.ThisPtr)->CopyResource(buffer.As<ID3D11Resource.Vftbl>(ID3D11Resource.IID), resource);
                    ((IDXGISwapChain1*)swapChain1.ThisPtr)->Present(0, default);
                }
            }
        }
    }
}
