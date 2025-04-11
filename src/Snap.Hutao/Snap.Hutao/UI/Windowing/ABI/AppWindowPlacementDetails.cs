// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using WinRT;
using WinRT.Interop;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace ABI.Microsoft.UI.Windowing;
#pragma warning restore IDE0130

[EditorBrowsable(EditorBrowsableState.Never)]
internal struct AppWindowPlacementDetails
{
    public static IObjectReference? CreateMarshaler(global::Microsoft.UI.Windowing.AppWindowPlacementDetails? obj)
    {
        return obj is null ? null : MarshalInspectable<global::Microsoft.UI.Windowing.AppWindowPlacementDetails>.CreateMarshaler<IUnknownVftbl>(obj, IAppWindowPlacementDetailsMethods.IID);
    }

    public static ObjectReferenceValue CreateMarshaler2(global::Microsoft.UI.Windowing.AppWindowPlacementDetails obj)
    {
        return MarshalInspectable<object>.CreateMarshaler2(obj, IAppWindowPlacementDetailsMethods.IID);
    }

    public static nint GetAbi(IObjectReference? value)
    {
        return value is null ? 0 : MarshalInterfaceHelper<object>.GetAbi(value);
    }

    public static global::Microsoft.UI.Windowing.AppWindowPlacementDetails FromAbi(nint thisPtr)
    {
        return global::Microsoft.UI.Windowing.AppWindowPlacementDetails.FromAbi(thisPtr);
    }

    public static nint FromManaged(global::Microsoft.UI.Windowing.AppWindowPlacementDetails? obj)
    {
        return obj is null ? 0 : CreateMarshaler2(obj).Detach();
    }

    public static MarshalInterfaceHelper<global::Microsoft.UI.Windowing.AppWindowPlacementDetails>.MarshalerArray CreateMarshalerArray(global::Microsoft.UI.Windowing.AppWindowPlacementDetails[] array)
    {
        return MarshalInterfaceHelper<global::Microsoft.UI.Windowing.AppWindowPlacementDetails>.CreateMarshalerArray2(array, CreateMarshaler2);
    }

    public static (int Length, nint Data) GetAbiArray(object box)
    {
        return MarshalInterfaceHelper<global::Microsoft.UI.Windowing.AppWindowPlacementDetails>.GetAbiArray(box);
    }

    public static global::Microsoft.UI.Windowing.AppWindowPlacementDetails[] FromAbiArray(object box)
    {
        return MarshalInterfaceHelper<global::Microsoft.UI.Windowing.AppWindowPlacementDetails>.FromAbiArray(box, FromAbi);
    }

    public static void CopyAbiArray(global::Microsoft.UI.Windowing.AppWindowPlacementDetails[] array, object box)
    {
        MarshalInterfaceHelper<global::Microsoft.UI.Windowing.AppWindowPlacementDetails>.CopyAbiArray(array, box, FromAbi);
    }

    public static (int Length, nint Data) FromManagedArray(global::Microsoft.UI.Windowing.AppWindowPlacementDetails[] array)
    {
        return MarshalInterfaceHelper<global::Microsoft.UI.Windowing.AppWindowPlacementDetails>.FromManagedArray(array, FromManaged);
    }

    public static void DisposeMarshaler(IObjectReference value)
    {
        MarshalInspectable<object>.DisposeMarshaler(value);
    }

    public static void DisposeMarshalerArray(MarshalInterfaceHelper<global::Microsoft.UI.Windowing.AppWindowPlacementDetails>.MarshalerArray array)
    {
        MarshalInterfaceHelper<global::Microsoft.UI.Windowing.AppWindowPlacementDetails>.DisposeMarshalerArray(array);
    }

    public static void DisposeAbi(nint abi)
    {
        MarshalInspectable<object>.DisposeAbi(abi);
    }

    public static void DisposeAbiArray(object box)
    {
        MarshalInspectable<object>.DisposeAbiArray(box);
    }
}