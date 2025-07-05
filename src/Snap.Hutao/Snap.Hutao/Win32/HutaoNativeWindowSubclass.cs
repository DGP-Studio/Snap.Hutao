// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeWindowSubclass
{
    private readonly ObjectReference<Vftbl2>? objRef2;

    public HutaoNativeWindowSubclass(ObjectReference<Vftbl> objRef)
    {
        ObjRef = objRef;
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
    }

    private ObjectReference<Vftbl> ObjRef { get; }

    private ObjectReference<Vftbl2>? ObjRef2 { get => objRef2; }

    public void Attach()
    {
        Marshal.ThrowExceptionForHR(ObjRef.Vftbl.Attach(ObjRef.ThisPtr));
    }

    public void Detach()
    {
        Marshal.ThrowExceptionForHR(ObjRef.Vftbl.Detach(ObjRef.ThisPtr));
    }

    public void InitializeTaskbarProgress()
    {
        HutaoException.NotSupportedIf(ObjRef2 is null, "IHutaoNativeWindowSubclass2 is not supported");
        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.InitializeTaskbarProgress(ObjRef2.ThisPtr));
    }

    public void SetTaskbarProgress(TBPFLAG flags, ulong value, ulong maximum)
    {
        HutaoException.NotSupportedIf(ObjRef2 is null, "IHutaoNativeWindowSubclass2 is not supported");
        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.SetTaskbarProgress(ObjRef2.ThisPtr, flags, value, maximum));
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeWindowSubclass)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Attach;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Detach;
#pragma warning restore CS0649
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeWindowSubclass2)]
    internal readonly struct Vftbl2
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> InitializeTaskbarProgress;
        internal readonly delegate* unmanaged[Stdcall]<nint, TBPFLAG, ulong, ulong, HRESULT> SetTaskbarProgress;
#pragma warning restore CS0649
    }
}