// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.UI.Input.HotKey;

internal sealed partial class HotKeyCombination : ObservableObject, IDisposable
{
    private readonly HutaoNativeHotKeyActionKind kind;
    private readonly string settingKey;

    private HutaoNativeHotKeyAction? native;
    private GCHandle handle;

    // IMPORTANT: DO NOT CONVERT TO AUTO PROPERTIES
    private bool modifierHasControl;
    private bool modifierHasShift;
    private bool modifierHasAlt;
    private NameValue<VIRTUAL_KEY> keyNameValue;
    private HOT_KEY_MODIFIERS modifiers;
    private bool isEnabled;
    private VIRTUAL_KEY key;

    public HotKeyCombination(HutaoNativeHotKeyActionKind kind, string settingKey)
    {
        this.kind = kind;
        this.settingKey = settingKey;

        // Initialize Property backing fields
        {
            // Retrieve from LocalSetting
            isEnabled = LocalSetting.Get($"{settingKey}.IsEnabled", true);

            HotKeyParameter parameter = HotKeyParameter.Default;
            long value = LocalSetting.Get(settingKey, Unsafe.As<HotKeyParameter, long>(ref parameter));
            HotKeyParameter actual = Unsafe.As<long, HotKeyParameter>(ref value);

            // HOT_KEY_MODIFIERS.MOD_WIN is reserved for use by the OS.
            // This line should keep exists, we allow user to set it long time ago.
            modifiers = actual.Modifiers & ~HOT_KEY_MODIFIERS.MOD_WIN;
            modifierHasControl = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL);
            modifierHasShift = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT);
            modifierHasAlt = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT);

            keyNameValue = VirtualKeys.HotKeyValues.SingleOrDefault(nk => nk.Value == actual.Key) ?? VirtualKeys.HotKeyValues.Last();
            key = keyNameValue.Value;
        }

        handle = GCHandle.Alloc(this);
    }

    public bool ModifierHasControl { get => modifierHasControl; set => _ = SetProperty(ref modifierHasControl, value) && UpdateModifiers(); }

    public bool ModifierHasShift { get => modifierHasShift; set => _ = SetProperty(ref modifierHasShift, value) && UpdateModifiers(); }

    public bool ModifierHasAlt { get => modifierHasAlt; set => _ = SetProperty(ref modifierHasAlt, value) && UpdateModifiers(); }

    [AllowNull]
    public NameValue<VIRTUAL_KEY> KeyNameValue
    {
        get => keyNameValue;
        set
        {
            if (value is not null && SetProperty(ref keyNameValue, value))
            {
                Key = value.Value;
            }
        }
    }

    public HOT_KEY_MODIFIERS Modifiers
    {
        get => modifiers;
        private set
        {
            if (SetProperty(ref modifiers, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                SaveModifiersAndKeyThenUpdate();
            }
        }
    }

    public VIRTUAL_KEY Key
    {
        get => key;
        private set
        {
            if (SetProperty(ref key, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                SaveModifiersAndKeyThenUpdate();
            }
        }
    }

    /// <summary>
    /// Can perform the action.
    /// </summary>
    public bool IsEnabled
    {
        get => native?.IsEnabled ?? false;
        set
        {
            if (native is not null)
            {
                native.IsEnabled = value;
                OnPropertyChanged();
            }

            LocalSetting.Set($"{settingKey}.IsEnabled", value);
        }
    }

    /// <summary>
    /// Is performing the action.
    /// </summary>
    [ObservableProperty]
    [UsedImplicitly]
    public partial bool IsOn { get; private set; }

    public string DisplayName { get => ToString(); }

    public unsafe void Initialize()
    {
        native = HutaoNative.Instance.MakeHotKeyAction(kind, HutaoNativeHotKeyActionCallback.Create(&OnAction), GCHandle.ToIntPtr(handle));
        native.Update(modifiers, (uint)key);
    }

    public void Dispose()
    {
        handle.Free();
        native = default;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();

        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL))
        {
            stringBuilder.Append("Ctrl").Append(" + ");
        }

        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT))
        {
            stringBuilder.Append("Shift").Append(" + ");
        }

        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT))
        {
            stringBuilder.Append("Alt").Append(" + ");
        }

        stringBuilder.Append(Key.ToString().AsSpan()[3..].Trim('_'));

        return stringBuilder.ToString();
    }

    private bool UpdateModifiers()
    {
        HOT_KEY_MODIFIERS modifiers = default;

        if (ModifierHasControl)
        {
            modifiers |= HOT_KEY_MODIFIERS.MOD_CONTROL;
        }

        if (ModifierHasShift)
        {
            modifiers |= HOT_KEY_MODIFIERS.MOD_SHIFT;
        }

        if (ModifierHasAlt)
        {
            modifiers |= HOT_KEY_MODIFIERS.MOD_ALT;
        }

        Modifiers = modifiers;
        return true;
    }

    private unsafe void SaveModifiersAndKeyThenUpdate()
    {
        HotKeyParameter current = new(Modifiers, Key);
        LocalSetting.Set(settingKey, *(long*)&current);
        native?.Update(modifiers, (uint)key);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void OnAction(BOOL isOn, nint data)
    {
        if (GCHandle.FromIntPtr(data).Target is not HotKeyCombination combination)
        {
            return;
        }

        combination.IsOn = isOn;
    }
}