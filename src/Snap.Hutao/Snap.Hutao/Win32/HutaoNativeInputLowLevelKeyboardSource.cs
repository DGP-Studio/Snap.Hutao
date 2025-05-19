// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeInputLowLevelKeyboardSource
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeInputLowLevelKeyboardSource(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public void Attach(HutaoNativeInputLowLevelKeyboardSourceCallback callback)
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Attach(objRef.ThisPtr, callback));
    }

    public void Detach(HutaoNativeInputLowLevelKeyboardSourceCallback callback)
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Detach(objRef.ThisPtr, callback));
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeInputLowLevelKeyboardSource)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeInputLowLevelKeyboardSourceCallback, HRESULT> Attach;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeInputLowLevelKeyboardSourceCallback, HRESULT> Detach;
#pragma warning restore CS0649
    }
}