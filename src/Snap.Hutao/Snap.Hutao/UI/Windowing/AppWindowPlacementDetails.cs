// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using ABI.Microsoft.UI.Windowing;
using Microsoft.Foundation;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT;
using WinRT.Interop;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace Microsoft.UI.Windowing;
#pragma warning restore IDE0130

[WindowsRuntimeType("Microsoft.UI")]
[WindowsRuntimeHelperType(typeof(global::ABI.Microsoft.UI.Windowing.AppWindowPlacementDetails))]
[AppWindowPlacementDetailsRcwFactory]
[ProjectedRuntimeClass(typeof(IAppWindowPlacementDetails))]
[global::Windows.Foundation.Metadata.ContractVersion(typeof(WindowsAppSDKContract), 65543u)]
internal sealed class AppWindowPlacementDetails : ICustomQueryInterface, IWinRTObject, IEquatable<AppWindowPlacementDetails>
{
    private static volatile IObjectReference? objRefMicrosoftUIWindowingIAppWindowPlacementDetailsStatics;

    private readonly IObjectReference inner;

    private volatile ConcurrentDictionary<RuntimeTypeHandle, IObjectReference>? queryInterfaceCache;
    private volatile ConcurrentDictionary<RuntimeTypeHandle, object>? additionalTypeData;

    internal AppWindowPlacementDetails(IObjectReference objRef)
    {
        inner = objRef.As(IAppWindowPlacementDetailsMethods.IID);
    }

    public RectInt32 ArrangeRect
    {
        get => IAppWindowPlacementDetailsMethods.get_ArrangeRect(ObjRefMicrosoftUIWindowingIAppWindowPlacementDetails);
    }

    public string DeviceName
    {
        get => IAppWindowPlacementDetailsMethods.get_DeviceName(ObjRefMicrosoftUIWindowingIAppWindowPlacementDetails);
    }

    public int Dpi
    {
        get => IAppWindowPlacementDetailsMethods.get_Dpi(ObjRefMicrosoftUIWindowingIAppWindowPlacementDetails);
    }

    public PlacementInfo Flags
    {
        get => IAppWindowPlacementDetailsMethods.get_Flags(ObjRefMicrosoftUIWindowingIAppWindowPlacementDetails);
    }

    public RectInt32 NormalRect
    {
        get => IAppWindowPlacementDetailsMethods.get_NormalRect(ObjRefMicrosoftUIWindowingIAppWindowPlacementDetails);
    }

    public int ShowCmd
    {
        get => IAppWindowPlacementDetailsMethods.get_ShowCmd(ObjRefMicrosoftUIWindowingIAppWindowPlacementDetails);
    }

    public RectInt32 WorkArea
    {
        get => IAppWindowPlacementDetailsMethods.get_WorkArea(ObjRefMicrosoftUIWindowingIAppWindowPlacementDetails);
    }

    bool IWinRTObject.HasUnwrappableNativeObject { get => true; }

    IObjectReference IWinRTObject.NativeObject { get => inner; }

    ConcurrentDictionary<RuntimeTypeHandle, IObjectReference> IWinRTObject.QueryInterfaceCache { get => queryInterfaceCache ?? MakeQueryInterfaceCache(); }

    ConcurrentDictionary<RuntimeTypeHandle, object> IWinRTObject.AdditionalTypeData { get => additionalTypeData ?? MakeAdditionalTypeData(); }

    private static IObjectReference ObjRefMicrosoftUIWindowingIAppWindowPlacementDetailsStatics
    {
        get
        {
            IObjectReference? factory = objRefMicrosoftUIWindowingIAppWindowPlacementDetailsStatics;
            return factory is not null && factory.IsInCurrentContext
                ? factory
                : objRefMicrosoftUIWindowingIAppWindowPlacementDetailsStatics = ActivationFactory.Get("Microsoft.UI.Windowing.AppWindowPlacementDetails", IAppWindowPlacementDetailsStaticsMethods.IID);
        }
    }

    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    private nint ThisPtr { get => inner?.ThisPtr ?? ((IWinRTObject)this).NativeObject.ThisPtr; }

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    private IObjectReference ObjRefMicrosoftUIWindowingIAppWindowPlacementDetails { get => inner; }

    public static bool operator ==(AppWindowPlacementDetails? x, AppWindowPlacementDetails? y)
    {
        return (x?.ThisPtr ?? 0) == (y?.ThisPtr ?? 0);
    }

    public static bool operator !=(AppWindowPlacementDetails? x, AppWindowPlacementDetails? y)
    {
        return !(x == y);
    }

    public static TInterface As<TInterface>()
    {
        return ActivationFactory.Get("Microsoft.UI.Windowing.AppWindowPlacementDetails").AsInterface<TInterface>();
    }

    public static AppWindowPlacementDetails Create(RectInt32 normalRect, RectInt32 workArea, int dpi, int showCmd, RectInt32 arrangeRect, PlacementInfo flags, string deviceName)
    {
        return IAppWindowPlacementDetailsStaticsMethods.Create(ObjRefMicrosoftUIWindowingIAppWindowPlacementDetailsStatics, normalRect, workArea, dpi, showCmd, arrangeRect, flags, deviceName);
    }

    public static AppWindowPlacementDetails FromAbi(nint thisPtr)
    {
        return thisPtr is 0 ? default! : MarshalInspectable<AppWindowPlacementDetails>.FromAbi(thisPtr);
    }

    public bool Equals(AppWindowPlacementDetails? other)
    {
        return this == other;
    }

    public override bool Equals(object? obj)
    {
        return obj is AppWindowPlacementDetails that && this == that;
    }

    public override int GetHashCode()
    {
        return ThisPtr.GetHashCode();
    }

    CustomQueryInterfaceResult ICustomQueryInterface.GetInterface(ref Guid iid, out nint ppv)
    {
        ppv = 0;
        if (IsOverridableInterface(iid) || IID.IID_IInspectable == iid)
        {
            return CustomQueryInterfaceResult.NotHandled;
        }

        if (((IWinRTObject)this).NativeObject.TryAs(iid, out ppv) >= 0)
        {
            return CustomQueryInterfaceResult.Handled;
        }

        return CustomQueryInterfaceResult.NotHandled;
    }

    private ConcurrentDictionary<RuntimeTypeHandle, IObjectReference> MakeQueryInterfaceCache()
    {
        Interlocked.CompareExchange(ref queryInterfaceCache, new(), null);
        return queryInterfaceCache;
    }

    private ConcurrentDictionary<RuntimeTypeHandle, object> MakeAdditionalTypeData()
    {
        Interlocked.CompareExchange(ref additionalTypeData, new(), null);
        return additionalTypeData;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    // ReSharper disable once UnusedParameter.Local
    private bool IsOverridableInterface(Guid iid)
    {
        return false;
    }

    // ReSharper disable once UnusedType.Local
    private struct InterfaceTag<TInterface>;
}