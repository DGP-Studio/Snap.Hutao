// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal unsafe struct IDXGIFactory6
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x4F, 0x69, 0xB6, 0xC1, 0x09, 0xFF, 0xA9, 0x44, 0xB0, 0x3C, 0x77, 0x90, 0x0A, 0x0A, 0x1D, 0x17];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public unsafe HRESULT CreateSwapChainForComposition(IObjectReference device, ref readonly DXGI_SWAP_CHAIN_DESC1 desc, ObjectReference<IDXGIOutput.Vftbl>? restrictToOutput, out ObjectReference<IDXGISwapChain1.Vftbl> swapChain)
    {
        fixed (DXGI_SWAP_CHAIN_DESC1* pDesc = &desc)
        {
            IDXGISwapChain1* pSwapChain = default;
            HRESULT hr = ThisPtr->IDXGIFactory5Vftbl
                .IDXGIFactory4Vftbl
                .IDXGIFactory3Vftbl
                .IDXGIFactory2Vftbl
                .CreateSwapChainForComposition((IDXGIFactory2*)Unsafe.AsPointer(ref this), (IUnknown*)device.ThisPtr, pDesc, (IDXGIOutput*)(restrictToOutput?.ThisPtr ?? 0), &pSwapChain);
            swapChain = ObjectReference<IDXGISwapChain1.Vftbl>.Attach(ref Unsafe.AsRef<nint>(&pSwapChain), IDXGISwapChain1.IID);
            return hr;
        }
    }

    [SuppressMessage("", "SA1313")]
    public HRESULT EnumAdapterByGpuPreference<TVftbl>(uint Adapter, DXGI_GPU_PREFERENCE GpuPreference, ref readonly Guid iid, out ObjectReference<TVftbl> vAdapter)
        where TVftbl : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            TVftbl** pvAdapter = default;
            HRESULT hr = ThisPtr->EnumAdapterByGpuPreference((IDXGIFactory6*)Unsafe.AsPointer(ref this), Adapter, GpuPreference, riid, (void**)&pvAdapter);
            vAdapter = ObjectReference<TVftbl>.Attach(ref Unsafe.AsRef<nint>(&pvAdapter), iid);
            return hr;
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIFactory5.Vftbl IDXGIFactory5Vftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory6*, uint, DXGI_GPU_PREFERENCE, Guid*, void**, HRESULT> EnumAdapterByGpuPreference;
    }
}