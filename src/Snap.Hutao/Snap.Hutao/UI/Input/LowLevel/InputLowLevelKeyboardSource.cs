// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;

namespace Snap.Hutao.UI.Input.LowLevel;

internal delegate void InputLowLevelKeyboardSourceEventHandler(LowLevelKeyEventArgs args);

internal static class InputLowLevelKeyboardSource
{
    public static event InputLowLevelKeyboardSourceEventHandler? KeyDown;

    public static event InputLowLevelKeyboardSourceEventHandler? KeyUp;

    public static event InputLowLevelKeyboardSourceEventHandler? SystemKeyDown;

    public static event InputLowLevelKeyboardSourceEventHandler? SystemKeyUp;

    [field: MaybeNull]
    private static HutaoNativeInputLowLevelKeyboardSource Native
    {
        get => LazyInitializer.EnsureInitialized(ref field, HutaoNative.Instance.MakeInputLowLevelKeyboardSource);
    }

    public static unsafe void Initialize()
    {
        Native.Attach(HutaoNativeInputLowLevelKeyboardSourceCallback.Create(&ProcessLowLevelKeyboard));
    }

    public static unsafe void Uninitialize()
    {
        Native.Detach(HutaoNativeInputLowLevelKeyboardSourceCallback.Create(&ProcessLowLevelKeyboard));
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe BOOL ProcessLowLevelKeyboard(uint param, KBDLLHOOKSTRUCT* data)
    {
        LowLevelKeyEventArgs args = new(*data);
        switch (param)
        {
            case WM_KEYDOWN:
                KeyDown?.Invoke(args);
                break;
            case WM_KEYUP:
                KeyUp?.Invoke(args);
                break;
            case WM_SYSKEYDOWN:
                SystemKeyDown?.Invoke(args);
                break;
            case WM_SYSKEYUP:
                SystemKeyUp?.Invoke(args);
                break;
        }

        return args.Handled;
    }
}