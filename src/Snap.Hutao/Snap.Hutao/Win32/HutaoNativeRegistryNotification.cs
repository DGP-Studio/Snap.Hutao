// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

[Guid("ef118e91-8ad9-4c27-997d-daf8910b34be")]
internal sealed unsafe class HutaoNativeRegistryNotification
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeRegistryNotification(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public void Start(HutaoNativeRegistryNotificationCallback callback, nint userData)
    {
        HutaoException.NotSupportedIf(objRef is null, "IHutaoNativeRegistryNotification.Start is not supported");
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Start(objRef.ThisPtr, callback, userData));
    }

    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeRegistryNotificationCallback, nint, HRESULT> Start;
#pragma warning restore CS0649
    }
}