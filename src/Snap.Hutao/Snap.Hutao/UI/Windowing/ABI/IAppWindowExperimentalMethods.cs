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
internal static class IAppWindowExperimentalMethods
{
    public static ref readonly Guid IID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.AsRef<Guid>([0xC7, 0x96, 0xDB, 0x4, 0xB6, 0xDE, 0xE4, 0x5B, 0xBF, 0xDC, 0x1B, 0xC0, 0x36, 0x1C, 0x8A, 0x12]);
    }

    internal static unsafe global::Microsoft.UI.Windowing.AppWindowPlacementDetails GetCurrentPlacement(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        nint retval = default;
        try
        {
            ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, nint*, int>**)thisPtr)[10](thisPtr, &retval));
            GC.KeepAlive(obj);
            return AppWindowPlacementDetails.FromAbi(retval);
        }
        finally
        {
            AppWindowPlacementDetails.DisposeAbi(retval);
        }
    }

    internal static unsafe void SaveCurrentPlacement(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, int>**)thisPtr)[11](thisPtr));
        GC.KeepAlive(obj);
    }

    internal static unsafe bool SetCurrentPlacement(IObjectReference obj, global::Microsoft.UI.Windowing.AppWindowPlacementDetails placementDetails, bool isFirstWindow)
    {
        nint thisPtr = obj.ThisPtr;

        ObjectReferenceValue placementDetailsValue = default;
        byte retval = default;
        try
        {
            placementDetailsValue = AppWindowPlacementDetails.CreateMarshaler2(placementDetails);
            ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, nint, byte, byte*, int>**)thisPtr)[12](thisPtr, MarshalInspectable<object>.GetAbi(placementDetailsValue), (byte)(isFirstWindow ? 1 : 0), &retval));
            GC.KeepAlive(obj);
            return retval != 0;
        }
        finally
        {
            MarshalInspectable<object>.DisposeMarshaler(placementDetailsValue);
        }
    }

    internal static unsafe Guid? get_PersistedStateId(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        nint retval = default;
        try
        {
            ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, nint*, int>**)thisPtr)[6](thisPtr, &retval));
            GC.KeepAlive(obj);
            return MarshalInterface<Guid?>.FromAbi(retval);
        }
        finally
        {
            MarshalInterface<Guid?>.DisposeAbi(retval);
        }
    }

    internal static unsafe void set_PersistedStateId(IObjectReference obj, Guid? value)
    {
        nint thisPtr = obj.ThisPtr;

        ObjectReferenceValue valueValue = default;
        try
        {
            valueValue = MarshalInterface<Guid?>.CreateMarshaler2(value, global::ABI.System.Nullable<Guid>.PIID);
            ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, nint, int>**)thisPtr)[7](thisPtr, MarshalInspectable<object>.GetAbi(valueValue)));
            GC.KeepAlive(obj);
        }
        finally
        {
            MarshalInspectable<object>.DisposeMarshaler(valueValue);
        }
    }

    internal static unsafe global::Microsoft.UI.Windowing.PlacementRestorationBehavior get_PlacementRestorationBehavior(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        global::Microsoft.UI.Windowing.PlacementRestorationBehavior retval = default;
        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Windowing.PlacementRestorationBehavior*, int>**)thisPtr)[8](thisPtr, &retval));
        GC.KeepAlive(obj);
        return retval;
    }

    internal static unsafe void set_PlacementRestorationBehavior(IObjectReference obj, global::Microsoft.UI.Windowing.PlacementRestorationBehavior value)
    {
        nint thisPtr = obj.ThisPtr;

        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Windowing.PlacementRestorationBehavior, int>**)thisPtr)[9](thisPtr, value));
        GC.KeepAlive(obj);
    }
}