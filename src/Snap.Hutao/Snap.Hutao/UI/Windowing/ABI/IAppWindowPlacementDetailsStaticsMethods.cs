// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace ABI.Microsoft.UI.Windowing;
#pragma warning restore IDE0130

internal static class IAppWindowPlacementDetailsStaticsMethods
{
    public static ref readonly Guid IID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.AsRef<Guid>([0x45, 0x17, 0x9F, 0xF1, 0xAD, 0x52, 0xF7, 0x5C, 0x97, 0xEA, 0x76, 0xC5, 0xFD, 0x6F, 0xF3, 0xC1]);
    }

    internal static unsafe global::Microsoft.UI.Windowing.AppWindowPlacementDetails Create(IObjectReference obj, global::Windows.Graphics.RectInt32 normalRect, global::Windows.Graphics.RectInt32 workArea, int dpi, int showCmd, global::Windows.Graphics.RectInt32 arrangeRect, global::Microsoft.UI.Windowing.PlacementInfo flags, string deviceName)
    {
        nint thisPtr = obj.ThisPtr;

        nint retval = default;
        try
        {
            MarshalString.Pinnable deviceNameValue = new(deviceName);
            fixed (void* pDeviceName = deviceNameValue)
            {
                ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<nint, global::Windows.Graphics.RectInt32, global::Windows.Graphics.RectInt32, int, int, global::Windows.Graphics.RectInt32, global::Microsoft.UI.Windowing.PlacementInfo, nint, nint*, int>**)thisPtr)[6](thisPtr, normalRect, workArea, dpi, showCmd, arrangeRect, flags, MarshalString.GetAbi(ref deviceNameValue), &retval));
                GC.KeepAlive(obj);
                return AppWindowPlacementDetails.FromAbi(retval);
            }
        }
        finally
        {
            AppWindowPlacementDetails.DisposeAbi(retval);
        }
    }
}