// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi;
using WinRT;

namespace Snap.Hutao.Win32.System.WinRT.Xaml;

internal static class ISwapChainPanelNativeExtension
{
    public static unsafe HRESULT SetSwapChain(this ISwapChainPanelNative native, ObjectReference<IDXGISwapChain.Vftbl>? swapChain)
    {
        return native.SetSwapChain(MarshalInterfaceHelper<object>.GetAbi(swapChain));
    }
}