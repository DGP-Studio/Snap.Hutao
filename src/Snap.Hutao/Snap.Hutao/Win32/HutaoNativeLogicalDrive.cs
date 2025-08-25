// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeLogicalDrive
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeLogicalDrive(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public long GetDiskFreeSpace(string path)
    {
        long bytes;
        fixed (char* pPath = path)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetDiskFreeSpace(objRef.ThisPtr, pPath, &bytes));
        }

        return bytes;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeLogicalDrive)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, long*, HRESULT> GetDiskFreeSpace;
#pragma warning restore CS0649
    }
}