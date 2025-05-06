// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

[Guid("9631921e-a6ca-4150-9939-99b5467b2fd6")]
internal sealed unsafe class HutaoNativeWindowSubclass
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeWindowSubclass(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public void Attach()
    {
        HutaoException.NotSupportedIf(objRef is null, "IHutaoNativeWindowSubclass.Attach is not supported");
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Attach(objRef.ThisPtr));
    }

    public void Detach()
    {
        HutaoException.NotSupportedIf(objRef is null, "IHutaoNativeWindowSubclass.Detach is not supported");
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Detach(objRef.ThisPtr));
    }

    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Attach;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Detach;
#pragma warning restore CS0649
    }
}