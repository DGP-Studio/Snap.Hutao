// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

    [SuppressMessage("", "SA1313")]
    public HRESULT EnumAdapterByGpuPreference<T>(uint Adapter, DXGI_GPU_PREFERENCE GpuPreference, ref readonly Guid iid, out T* pvAdapter)
        where T : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            fixed (T** ppvAdapter = &pvAdapter)
            {
                return ThisPtr->EnumAdapterByGpuPreference((IDXGIFactory6*)Unsafe.AsPointer(ref this), Adapter, GpuPreference, riid, (void**)ppvAdapter);
            }
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIFactory5.Vftbl IDXGIFactory5Vftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory6*, uint, DXGI_GPU_PREFERENCE, Guid*, void**, HRESULT> EnumAdapterByGpuPreference;
    }
}