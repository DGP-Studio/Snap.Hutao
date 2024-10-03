// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal static unsafe class IDXGIFactory6
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x4F, 0x69, 0xB6, 0xC1, 0x09, 0xFF, 0xA9, 0x44, 0xB0, 0x3C, 0x77, 0x90, 0x0A, 0x0A, 0x1D, 0x17]);
    }

    public static unsafe HRESULT CreateSwapChainForComposition(this ObjectReference<Vftbl> objRef, IObjectReference device, ref readonly DXGI_SWAP_CHAIN_DESC1 desc, ObjectReference<IDXGIOutput.Vftbl>? restrictToOutput, out ObjectReference<IDXGISwapChain1.Vftbl> swapChain)
    {
        fixed (DXGI_SWAP_CHAIN_DESC1* pDesc = &desc)
        {
            nint pSwapChain = default;
            HRESULT hr = objRef.Vftbl
                .IDXGIFactory5Vftbl
                .IDXGIFactory4Vftbl
                .IDXGIFactory3Vftbl
                .IDXGIFactory2Vftbl
                .CreateSwapChainForComposition(objRef.ThisPtr, device.ThisPtr, pDesc, restrictToOutput?.ThisPtr ?? 0, &pSwapChain);
            swapChain = ObjectReference<IDXGISwapChain1.Vftbl>.Attach(ref pSwapChain, IDXGISwapChain1.IID);
            return hr;
        }
    }

    [SuppressMessage("", "SA1313")]
    public static HRESULT EnumAdapterByGpuPreference<TVftbl>(this ObjectReference<Vftbl> objRef, uint Adapter, DXGI_GPU_PREFERENCE GpuPreference, ref readonly Guid iid, out ObjectReference<TVftbl> vAdapter)
        where TVftbl : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            nint pvAdapter = default;
            HRESULT hr = objRef.Vftbl.EnumAdapterByGpuPreference(objRef.ThisPtr, Adapter, GpuPreference, riid, (void**)&pvAdapter);
            vAdapter = ObjectReference<TVftbl>.Attach(ref pvAdapter, iid);
            return hr;
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIFactory5.Vftbl IDXGIFactory5Vftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, DXGI_GPU_PREFERENCE, Guid*, void**, HRESULT> EnumAdapterByGpuPreference;
    }
}