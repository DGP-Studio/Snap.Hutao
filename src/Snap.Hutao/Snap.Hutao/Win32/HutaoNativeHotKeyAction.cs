// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeHotKeyAction
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeHotKeyAction(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public BOOL IsEnabled
    {
        get
        {
            BOOL isEnabled = default;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetIsEnabled(objRef.ThisPtr, &isEnabled));
            return isEnabled;
        }

        set
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.SetIsEnabled(objRef.ThisPtr, value));
        }
    }

    public static void InitializeBeforeSwitchCallback(HutaoNativeHotKeyBeforeSwitchCallback callback)
    {
        Marshal.ThrowExceptionForHR(HutaoNativeHotKeyInitializeBeforeSwitchCallback(callback));
    }

    public void Update(HOT_KEY_MODIFIERS modifiers, uint vk)
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Update(objRef.ThisPtr, modifiers, vk));
    }

    [DllImport(HutaoNativeMethods.DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT HutaoNativeHotKeyInitializeBeforeSwitchCallback(HutaoNativeHotKeyBeforeSwitchCallback callback);

    [Guid(HutaoNativeMethods.IID_IHutaoNativeHotKeyAction)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HOT_KEY_MODIFIERS, uint, HRESULT> Update;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> GetIsEnabled;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL, HRESULT> SetIsEnabled;
#pragma warning restore CS0649
    }
}