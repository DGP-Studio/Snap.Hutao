// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.CompilerServices;
using System.Text;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Input.HotKey;

internal sealed partial class HotKeyCombination : ObservableObject, IDisposable
{
    private readonly IInfoBarService infoBarService;

    private readonly Lock syncRoot = new();
    private readonly HWND hwnd;
    private readonly string settingKey;
    private readonly int hotKeyId;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly HotKeyParameter parameter;

    private CancellationTokenSource? cts = new();
    private bool registered;

    // IMPORTANT: DO NOT CONVERT TO AUTO PROPERTIES
    private bool modifierHasControl;
    private bool modifierHasShift;
    private bool modifierHasAlt;
    private NameValue<VIRTUAL_KEY> keyNameValue;
    private HOT_KEY_MODIFIERS modifiers;
    private bool isEnabled;
    private VIRTUAL_KEY key;

    public HotKeyCombination(IServiceProvider serviceProvider, HWND hwnd, string settingKey, int hotKeyId)
    {
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        this.hwnd = hwnd;
        this.settingKey = settingKey;
        this.hotKeyId = hotKeyId;
        parameter = new(default, VIRTUAL_KEY.VK__none_);

        // Initialize Property backing fields
        {
            // Retrieve from LocalSetting
            isEnabled = LocalSetting.Get($"{settingKey}.IsEnabled", true);

            int value = LocalSetting.Get(settingKey, Unsafe.As<HotKeyParameter, int>(ref parameter));
            HotKeyParameter actual = Unsafe.As<int, HotKeyParameter>(ref value);

            // HOT_KEY_MODIFIERS.MOD_WIN is reserved for use by the OS.
            // This line should keep exists, we allow user to set it long time ago.
            modifiers = actual.Modifiers & ~HOT_KEY_MODIFIERS.MOD_WIN;
            modifierHasControl = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL);
            modifierHasShift = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT);
            modifierHasAlt = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT);

            keyNameValue = VirtualKeys.HotKeyValues.SingleOrDefault(nk => nk.Value == actual.Key) ?? VirtualKeys.HotKeyValues.Last();
            key = keyNameValue.Value;
        }
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
                LocalSettingSetHotKeyParameterAndRefresh();
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

    [ObservableProperty]
    [UsedImplicitly]
    public partial bool IsOn { get; set; }

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

        registered = RegisterHotKey(hwnd, hotKeyId, Modifiers, (uint)Key);

        if (!registered)
        {
            infoBarService.Warning(SH.FormatCoreWindowHotkeyCombinationRegisterFailed(SH.ViewPageSettingKeyShortcutAutoClickingHeader, DisplayName));
        }

        return registered;
    }

    public void Dispose()
    {
        cts?.Cancel();
        cts?.Dispose();

        Unregister();
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

    internal void Toggle(WaitCallback callback)
    {
        lock (syncRoot)
        {
            if (IsOn)
            {
                // Turn off
                cts?.Cancel();
                cts = default;
                IsOn = false;
            }
            else
            {
                // Turn on
                cts = new();
                ThreadPool.QueueUserWorkItem(callback, cts.Token);
                IsOn = true;
            }
        }
    }

    private bool Unregister()
    {
        if (!HutaoRuntime.IsProcessElevated)
        {
            return false;
        }

        if (!registered)
        {
            return true;
        }

        registered = !UnregisterHotKey(hwnd, hotKeyId);
        return registered;
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