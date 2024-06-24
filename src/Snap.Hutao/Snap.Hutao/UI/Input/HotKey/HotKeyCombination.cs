// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Text;
using Windows.System;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Input.HotKey;

[SuppressMessage("", "SA1124")]
internal sealed class HotKeyCombination : ObservableObject
{
    private readonly IInfoBarService infoBarService;
    private readonly RuntimeOptions runtimeOptions;

    private readonly HWND hwnd;
    private readonly string settingKey;
    private readonly int hotKeyId;
    private readonly HotKeyParameter defaultHotKeyParameter;

    private bool registered;

    private bool modifierHasControl;
    private bool modifierHasShift;
    private bool modifierHasAlt;
    private NameValue<VirtualKey> keyNameValue;
    private HOT_KEY_MODIFIERS modifiers;
    private VirtualKey key;
    private bool isEnabled;
    private bool isOn;

    [SuppressMessage("", "SH002")]
    public HotKeyCombination(IServiceProvider serviceProvider, HWND hwnd, string settingKey, int hotKeyId, HOT_KEY_MODIFIERS defaultModifiers, VirtualKey defaultKey)
    {
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();

        this.hwnd = hwnd;
        this.settingKey = settingKey;
        this.hotKeyId = hotKeyId;
        defaultHotKeyParameter = new(defaultModifiers, defaultKey);

        // Initialize Property backing fields
        {
            // Retrieve from LocalSetting
            isEnabled = LocalSetting.Get($"{settingKey}.IsEnabled", true);

            HotKeyParameter actual = LocalSettingGetHotKeyParameter();

            // HOT_KEY_MODIFIERS.MOD_WIN is reversed for use by the OS.
            // It should not be used by the application.
            modifiers = actual.Modifiers & ~HOT_KEY_MODIFIERS.MOD_WIN;
            InitializeModifiersCompositionFields();
            key = actual.Key;

            keyNameValue = VirtualKeys.GetList().Single(v => v.Value == key);
        }
    }

    #region Binding Property
    public bool ModifierHasControl
    {
        get => modifierHasControl;
        set
        {
            if (SetProperty(ref modifierHasControl, value))
            {
                UpdateModifiers();
            }
        }
    }

    public bool ModifierHasShift
    {
        get => modifierHasShift;
        set
        {
            if (SetProperty(ref modifierHasShift, value))
            {
                UpdateModifiers();
            }
        }
    }

    public bool ModifierHasAlt
    {
        get => modifierHasAlt;
        set
        {
            if (SetProperty(ref modifierHasAlt, value))
            {
                UpdateModifiers();
            }
        }
    }

    public NameValue<VirtualKey> KeyNameValue
    {
        get => keyNameValue;
        set
        {
            if (value is null)
            {
                return;
            }

            if (SetProperty(ref keyNameValue, value))
            {
                Key = value.Value;
            }
        }
    }
    #endregion

    public HOT_KEY_MODIFIERS Modifiers
    {
        get => modifiers;
        private set
        {
            if (SetProperty(ref modifiers, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                LocalSettingSetHotKeyParameterAndRefresh();
            }
        }
    }

    public VirtualKey Key
    {
        get => key;
        private set
        {
            if (SetProperty(ref key, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                LocalSettingSetHotKeyParameterAndRefresh();
            }
        }
    }

    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (SetProperty(ref isEnabled, value))
            {
                LocalSetting.Set($"{settingKey}.IsEnabled", value);

                _ = (value, registered) switch
                {
                    (true, false) => Register(),
                    (false, true) => Unregister(),
                    _ => false,
                };
            }
        }
    }

    public bool IsOn
    {
        get => isOn;
        set => SetProperty(ref isOn, value);
    }

    public string DisplayName { get => ToString(); }

    public bool Register()
    {
        if (!runtimeOptions.IsElevated || !IsEnabled)
        {
            return false;
        }

        if (registered)
        {
            return true;
        }

        BOOL result = RegisterHotKey(hwnd, hotKeyId, Modifiers, (uint)Key);
        registered = result;

        if (!result)
        {
            infoBarService.Warning(SH.FormatCoreWindowHotkeyCombinationRegisterFailed(SH.ViewPageSettingKeyShortcutAutoClickingHeader, DisplayName));
        }

        return result;
    }

    public bool Unregister()
    {
        if (!runtimeOptions.IsElevated)
        {
            return false;
        }

        if (!registered)
        {
            return true;
        }

        BOOL result = UnregisterHotKey(hwnd, hotKeyId);
        registered = !result;
        return result;
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

        stringBuilder.Append(Key);

        return stringBuilder.ToString();
    }

    private void UpdateModifiers()
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
    }

    private void InitializeModifiersCompositionFields()
    {
        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL))
        {
            modifierHasControl = true;
        }

        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT))
        {
            modifierHasShift = true;
        }

        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT))
        {
            modifierHasAlt = true;
        }
    }

    private unsafe HotKeyParameter LocalSettingGetHotKeyParameter()
    {
        fixed (HotKeyParameter* pDefaultHotKey = &defaultHotKeyParameter)
        {
            int value = LocalSetting.Get(settingKey, *(int*)pDefaultHotKey);
            return *(HotKeyParameter*)&value;
        }
    }

    private unsafe void LocalSettingSetHotKeyParameterAndRefresh()
    {
        HotKeyParameter current = new(Modifiers, Key);
        LocalSetting.Set(settingKey, *(int*)&current);

        Unregister();
        Register();
    }
}