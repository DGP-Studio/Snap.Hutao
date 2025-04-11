// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace ABI.Microsoft.UI.Windowing;
#pragma warning restore IDE0130

[SuppressMessage("", "SA1300")]
[SuppressMessage("", "IDE1006")]
internal static class IAppWindowPlacementDetailsMethods
{
    public static ref readonly Guid IID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.AsRef<Guid>([0xB2, 0xC5, 0x9E, 0x63, 0xC, 0xAC, 0xBF, 0x5B, 0x84, 0x22, 0x98, 0xDC, 0xA5, 0x40, 0xD2, 0x19]);
    }

    internal static unsafe global::Windows.Graphics.RectInt32 get_ArrangeRect(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        global::Windows.Graphics.RectInt32 retval = default;
        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, global::Windows.Graphics.RectInt32*, int>**)thisPtr)[10](thisPtr, &retval));
        GC.KeepAlive(obj);
        return retval;
    }

    internal static unsafe string get_DeviceName(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        nint retval = default;
        try
        {
            ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, nint*, int>**)thisPtr)[11](thisPtr, &retval));
            GC.KeepAlive(obj);
            return MarshalString.FromAbi(retval);
        }
        finally
        {
            MarshalString.DisposeAbi(retval);
        }
    }

    internal static unsafe int get_Dpi(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        int retval = default;
        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, int*, int>**)thisPtr)[8](thisPtr, &retval));
        GC.KeepAlive(obj);
        return retval;
    }

    internal static unsafe global::Microsoft.UI.Windowing.PlacementInfo get_Flags(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        global::Microsoft.UI.Windowing.PlacementInfo retval = default;
        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Windowing.PlacementInfo*, int>**)thisPtr)[12](thisPtr, &retval));
        GC.KeepAlive(obj);
        return retval;
    }

    internal static unsafe global::Windows.Graphics.RectInt32 get_NormalRect(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        global::Windows.Graphics.RectInt32 retval = default;
        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, global::Windows.Graphics.RectInt32*, int>**)thisPtr)[6](thisPtr, &retval));
        GC.KeepAlive(obj);
        return retval;
    }

    internal static unsafe int get_ShowCmd(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        int retval = default;
        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, int*, int>**)thisPtr)[9](thisPtr, &retval));
        GC.KeepAlive(obj);
        return retval;
    }

    internal static unsafe global::Windows.Graphics.RectInt32 get_WorkArea(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        global::Windows.Graphics.RectInt32 retval = default;
        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, global::Windows.Graphics.RectInt32*, int>**)thisPtr)[7](thisPtr, &retval));
        GC.KeepAlive(obj);
        return retval;
    }
}