// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoString
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoString(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public string? Value
    {
        get
        {
            PCWSTR buffer;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetBuffer(objRef.ThisPtr, &buffer));
            return buffer.Value is null ? null : MemoryMarshal.CreateReadOnlySpanFromNullTerminated(buffer).ToString();
        }
    }

    public static HutaoString AttachAbi(ref nint abi)
    {
        return new(ObjectReference<Vftbl>.Attach(ref abi, typeof(Vftbl).GUID));
    }

    [Guid(HutaoNativeMethods.IID_IHutaoString)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR*, HRESULT> GetBuffer;
#pragma warning restore CS0649
    }
}