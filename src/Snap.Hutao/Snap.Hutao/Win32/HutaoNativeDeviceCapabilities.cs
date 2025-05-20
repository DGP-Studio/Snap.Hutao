// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeDeviceCapabilities
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeDeviceCapabilities(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public int GetPrimaryScreenVerticalRefreshRate()
    {
        int refreshRate = 0;
        Marshal.ThrowExceptionForHR(objRef.Vftbl.GetPrimaryScreenVerticalRefreshRate(objRef.ThisPtr, &refreshRate));
        return refreshRate;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeDeviceCapabilities)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, int*, HRESULT> GetPrimaryScreenVerticalRefreshRate;
#pragma warning restore CS0649
    }
}