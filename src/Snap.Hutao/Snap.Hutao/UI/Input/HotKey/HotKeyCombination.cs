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
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Input.HotKey;

internal sealed partial class HotKeyCombination : ObservableObject
{
    private readonly IInfoBarService infoBarService;

    private readonly HWND hwnd;
    private readonly string settingKey;
    private readonly int hotKeyId;
    private readonly HotKeyParameter defaultHotKeyParameter;

    private bool registered;

    public unsafe HotKeyCombination(IServiceProvider serviceProvider, HWND hwnd, string settingKey, int hotKeyId, HOT_KEY_MODIFIERS defaultModifiers, VIRTUAL_KEY defaultKey)
    {
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        this.hwnd = hwnd;
        this.settingKey = settingKey;
        this.hotKeyId = hotKeyId;
        defaultHotKeyParameter = new(defaultModifiers, defaultKey);

        // Initialize Property backing fields
        {
            // Retrieve from LocalSetting
            IsEnabled = LocalSetting.Get($"{settingKey}.IsEnabled", true);

            HotKeyParameter actual;
            fixed (HotKeyParameter* pDefaultHotKey = &defaultHotKeyParameter)
            {
                int value = LocalSetting.Get(settingKey, *(int*)pDefaultHotKey);
                actual = *(HotKeyParameter*)&value;
            }

            // HOT_KEY_MODIFIERS.MOD_WIN is reversed for use by the OS.
            // It should not be used by the application.
            Modifiers = actual.Modifiers & ~HOT_KEY_MODIFIERS.MOD_WIN;

            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL))
            {
                ModifierHasControl = true;
            }

            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT))
            {
                ModifierHasShift = true;
            }

            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT))
            {
                ModifierHasAlt = true;
            }

            Key = Enum.IsDefined(actual.Key) ? actual.Key : defaultKey;

            KeyNameValue = VirtualKeys.GetList().Single(v => v.Value == Key);
        }
    }

    public bool ModifierHasControl { get; set => _ = SetProperty(ref field, value) && UpdateModifiers(); }

    public bool ModifierHasShift { get; set => _ = SetProperty(ref field, value) && UpdateModifiers(); }

    public bool ModifierHasAlt { get; set => _ = SetProperty(ref field, value) && UpdateModifiers(); }

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

    public HOT_KEY_MODIFIERS Modifiers
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                LocalSettingSetHotKeyParameterAndRefresh();
            }
        }
    }

    public VIRTUAL_KEY Key
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                LocalSettingSetHotKeyParameterAndRefresh();
            }
        }
    }

    public bool IsEnabled
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
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

    public bool IsOn { get; set => SetProperty(ref field, value); }

    public string DisplayName { get => ToString(); }

    public bool Register()
    {
        if (!HutaoRuntime.IsProcessElevated || !IsEnabled)
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
        if (!HutaoRuntime.IsProcessElevated)
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

    private unsafe void LocalSettingSetHotKeyParameterAndRefresh()
    {
        HotKeyParameter current = new(Modifiers, Key);
        LocalSetting.Set(settingKey, *(int*)&current);

        Unregister();
        Register();
    }
}