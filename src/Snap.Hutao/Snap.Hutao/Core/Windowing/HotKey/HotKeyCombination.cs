// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using System.Text;
using Windows.System;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing.HotKey;

[SuppressMessage("", "SA1124")]
internal sealed class HotKeyCombination : ObservableObject
{
    private readonly ICurrentWindowReference currentWindowReference;
    private readonly RuntimeOptions runtimeOptions;

    private readonly string settingKey;
    private readonly int hotKeyId;
    private readonly HotKeyParameter defaultHotKeyParameter;

    private bool registered;

    private bool modifierHasWindows;
    private bool modifierHasControl;
    private bool modifierHasShift;
    private bool modifierHasAlt;
    private NameValue<VirtualKey> keyNameValue;
    private HOT_KEY_MODIFIERS modifiers;
    private VirtualKey key;
    private bool isEnabled;

    public HotKeyCombination(IServiceProvider serviceProvider, string settingKey, int hotKeyId, HOT_KEY_MODIFIERS defaultModifiers, VirtualKey defaultKey)
    {
        currentWindowReference = serviceProvider.GetRequiredService<ICurrentWindowReference>();
        runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();

        this.settingKey = settingKey;
        this.hotKeyId = hotKeyId;
        defaultHotKeyParameter = new(defaultModifiers, defaultKey);

        // Initialize Property backing fields
        {
            // Retrieve from LocalSetting
            isEnabled = LocalSetting.Get($"{settingKey}.IsEnabled", true);

            HotKeyParameter actual = LocalSettingGetHotKeyParameter();
            modifiers = actual.Modifiers;
            InitializeModifiersComposeFields();
            key = actual.Key;

            keyNameValue = VirtualKeys.GetList().Single(v => v.Value == key);
        }
    }

    #region Binding Property
    public bool ModifierHasWindows
    {
        get => modifierHasWindows;
        set
        {
            if (SetProperty(ref modifierHasWindows, value))
            {
                UpdateModifiers();
            }
        }
    }

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
                    (true, false) => RegisterForCurrentWindow(),
                    (false, true) => UnregisterForCurrentWindow(),
                    _ => false,
                };
            }
        }
    }

    public string DisplayName { get => ToString(); }

    public bool RegisterForCurrentWindow()
    {
        if (!runtimeOptions.IsElevated || !IsEnabled)
        {
            return false;
        }

        if (registered)
        {
            return true;
        }

        HWND hwnd = currentWindowReference.GetWindowHandle();
        BOOL result = RegisterHotKey(hwnd, hotKeyId, Modifiers, (uint)Key);
        registered = result;
        return result;
    }

    public bool UnregisterForCurrentWindow()
    {
        if (!runtimeOptions.IsElevated)
        {
            return false;
        }

        if (!registered)
        {
            return true;
        }

        HWND hwnd = currentWindowReference.GetWindowHandle();
        BOOL result = UnregisterHotKey(hwnd, hotKeyId);
        registered = !result;
        return result;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();

        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_WIN))
        {
            stringBuilder.Append("Win").Append(" + ");
        }

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

        if (ModifierHasWindows)
        {
            modifiers |= HOT_KEY_MODIFIERS.MOD_WIN;
        }

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

    private void InitializeModifiersComposeFields()
    {
        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_WIN))
        {
            modifierHasWindows = true;
        }

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

        UnregisterForCurrentWindow();
        RegisterForCurrentWindow();
    }
}