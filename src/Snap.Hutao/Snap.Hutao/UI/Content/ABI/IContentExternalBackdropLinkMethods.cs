// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using ABI.Microsoft.UI.Composition;
using ABI.Microsoft.UI.Dispatching;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace ABI.Microsoft.UI.Content;
#pragma warning restore IDE0130

[SuppressMessage("", "SA1300")]
[SuppressMessage("", "IDE1006")]
internal static class IContentExternalBackdropLinkMethods
{
    public static ref readonly Guid IID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.AsRef<Guid>([0x83, 0xBF, 0x54, 0x10, 0x5B, 0xB3, 0xDE, 0x5F, 0x8D, 0xD7, 0xAC, 0x3B, 0xB3, 0xE6, 0xCE, 0x27]);
    }

    internal static unsafe global::Microsoft.UI.Dispatching.DispatcherQueue get_DispatcherQueue(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        nint retval = default;
        try
        {
            ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, nint*, int>**)thisPtr)[6](thisPtr, &retval));
            GC.KeepAlive(obj);
            return DispatcherQueue.FromAbi(retval);
        }
        finally
        {
            DispatcherQueue.DisposeAbi(retval);
        }
    }

    internal static unsafe global::Microsoft.UI.Composition.CompositionBorderMode get_ExternalBackdropBorderMode(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        global::Microsoft.UI.Composition.CompositionBorderMode retval = default;
        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Composition.CompositionBorderMode*, int>**)thisPtr)[7](thisPtr, &retval));
        GC.KeepAlive(obj);
        return retval;
    }

    internal static unsafe void set_ExternalBackdropBorderMode(IObjectReference obj, global::Microsoft.UI.Composition.CompositionBorderMode value)
    {
        nint thisPtr = obj.ThisPtr;

        ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Composition.CompositionBorderMode, int>**)thisPtr)[8](thisPtr, value));
        GC.KeepAlive(obj);
    }

    internal static unsafe global::Microsoft.UI.Composition.Visual get_PlacementVisual(IObjectReference obj)
    {
        nint thisPtr = obj.ThisPtr;

        nint retval = default;
        try
        {
            ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, nint*, int>**)thisPtr)[9](thisPtr, &retval));
            GC.KeepAlive(obj);
            return Visual.FromAbi(retval);
        }
        finally
        {
            Visual.DisposeAbi(retval);
        }
    }
}