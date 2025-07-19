// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.UI.Input.HotKey;

internal sealed partial class HotKeyCombination : ObservableObject, IDisposable
{
    private readonly IInfoBarService infoBarService;
    private readonly HutaoNativeHotKeyActionKind kind;
    private readonly string settingKey;
    private readonly nint handle;

    private HutaoNativeHotKeyAction? native;

    public HotKeyCombination(IServiceProvider serviceProvider, HutaoNativeHotKeyActionKind kind, string settingKey)
    {
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        this.kind = kind;
        this.settingKey = settingKey;

        // Initialize Property backing fields
        {
            HotKeyParameter parameter = HotKeyParameter.Default;
            long value = LocalSetting.Get(settingKey, Unsafe.As<HotKeyParameter, long>(ref parameter));
            HotKeyParameter actual = Unsafe.As<long, HotKeyParameter>(ref value);

            // HOT_KEY_MODIFIERS.MOD_WIN is reserved for use by the OS.
            // This line should keep exists, we allow user to set it long time ago.
            FieldRefOfModifiers(this) = actual.Modifiers & ~HOT_KEY_MODIFIERS.MOD_WIN;
            FieldRefOfModifierHasControl(this) = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL);
            FieldRefOfModifierHasShift(this) = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT);
            FieldRefOfModifierHasAlt(this) = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT);

            FieldRefOfKeyNameValue(this) = VirtualKeys.HotKeyValues.SingleOrDefault(nk => nk.Value == actual.Key) ?? VirtualKeys.HotKeyValues.Last();
            FieldRefOfKey(this) = KeyNameValue!.Value;
        }

        handle = GCHandle.ToIntPtr(GCHandle.Alloc(this));
    }

    [FieldAccess]
    public bool ModifierHasControl { get; set => _ = SetProperty(ref field, value) && UpdateModifiers(); }

    [FieldAccess]
    public bool ModifierHasShift { get; set => _ = SetProperty(ref field, value) && UpdateModifiers(); }

    [FieldAccess]
    public bool ModifierHasAlt { get; set => _ = SetProperty(ref field, value) && UpdateModifiers(); }

    [AllowNull]
    [FieldAccess]
    public NameValue<VIRTUAL_KEY> KeyNameValue
    {
        get;
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                Key = value.Value;
            }
        }
    }

    [FieldAccess]
    public HOT_KEY_MODIFIERS Modifiers
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                SaveAndUpdate();
            }
        }
    }

    [FieldAccess]
    public VIRTUAL_KEY Key
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                SaveAndUpdate();
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
        native = HutaoNative.Instance.MakeHotKeyAction(kind, HutaoNativeHotKeyActionCallback.Create(&OnAction), handle);
        SaveAndUpdate();
    }

    public void Dispose()
    {
        GCHandle.FromIntPtr(handle).Free();
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

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void OnAction(BOOL isOn, nint data)
    {
        if (GCHandle.FromIntPtr(data).Target is not HotKeyCombination combination)
        {
            return;
        }

        combination.IsOn = isOn;
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

    private unsafe void SaveAndUpdate()
    {
        HotKeyParameter current = new(Modifiers, Key);
        LocalSetting.Set(settingKey, *(long*)&current);

        try
        {
            native?.Update(Modifiers, (uint)Key);
        }
        catch (COMException ex)
        {
            if (HutaoNative.IsWin32(ex.ErrorCode, WIN32_ERROR.ERROR_HOTKEY_ALREADY_REGISTERED))
            {
                infoBarService.Warning(SH.FormatCoreWindowHotkeyCombinationRegisterFailed(kind, DisplayName));
            }
            else
            {
                infoBarService.Error(ex);
            }
        }
    }
}