// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativePhysicalDrive
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativePhysicalDrive(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public bool IsPathOnSolidStateDrive(string root)
    {
        BOOL isSSD;
        fixed (char* pRoot = root)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.IsPathOnSolidStateDrive(objRef.ThisPtr, pRoot, &isSSD));
        }

        return isSSD;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativePhysicalDrive)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, BOOL*, HRESULT> IsPathOnSolidStateDrive;
#pragma warning restore CS0649
    }
}