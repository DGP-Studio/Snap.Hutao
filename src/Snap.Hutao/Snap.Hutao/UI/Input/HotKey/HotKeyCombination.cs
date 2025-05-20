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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Input.HotKey;

internal sealed partial class HotKeyCombination : ObservableObject, IDisposable
{
    private static readonly ConcurrentDictionary<HotKeyParameter, Void> RegisteredHotKeys = [];

    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private readonly Lock syncRoot = new();
    private readonly HWND messageHwnd;
    private readonly string name;
    private readonly string settingKey;
    private readonly int hotKeyId;

#pragma warning disable CA2213
    // Make volatile to prevent the callback getting a null cts.
    private volatile CancellationTokenSource? cts = new();
#pragma warning restore CA2213

    private bool registered;

    // IMPORTANT: DO NOT CONVERT TO AUTO PROPERTIES
    private bool modifierHasControl;
    private bool modifierHasShift;
    private bool modifierHasAlt;
    private NameValue<VIRTUAL_KEY> keyNameValue;
    private HOT_KEY_MODIFIERS modifiers;
    private bool isEnabled;
    private VIRTUAL_KEY key;

    public HotKeyCombination(IServiceProvider serviceProvider, HWND messageHwnd, string name, string settingKey, int hotKeyId)
    {
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        this.messageHwnd = messageHwnd;
        this.name = name;
        this.settingKey = settingKey;
        this.hotKeyId = hotKeyId;

        // Initialize Property backing fields
        {
            // Retrieve from LocalSetting
            isEnabled = LocalSetting.Get($"{settingKey}.IsEnabled", true);

            HotKeyParameter parameter = new(default, VIRTUAL_KEY.VK__none_);
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
                SaveModifiersAndKeyThenRefresh();
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
                SaveModifiersAndKeyThenRefresh();
            }
        }
    }

    /// <summary>
    /// Can perform the action.
    /// </summary>
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

    /// <summary>
    /// Is performing the action.
    /// </summary>
    [ObservableProperty]
    [UsedImplicitly]
    public partial bool IsOn { get; private set; }

    public string DisplayName { get => ToString(); }

    private HotKeyParameter Parameter { get => new(Modifiers, Key); }

    public bool Register()
    {
        if (!HutaoRuntime.IsProcessElevated || !IsEnabled || key is VIRTUAL_KEY.VK__none_)
        {
            return false;
        }

        if (registered)
        {
            return true;
        }

        if (RegisteredHotKeys.TryGetValue(Parameter, out _))
        {
            return false;
        }

        registered = RegisterHotKey(messageHwnd, hotKeyId, Modifiers, (uint)Key);

        if (registered)
        {
            RegisteredHotKeys.TryAdd(Parameter, default);
        }
        else
        {
            infoBarService.Warning(SH.FormatCoreWindowHotkeyCombinationRegisterFailed(name, DisplayName));
        }

        return registered;
    }

    public void Dispose()
    {
        cts?.Cancel();

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

        stringBuilder.Append(Key.ToString().AsSpan()[3..].Trim('_'));

        return stringBuilder.ToString();
    }

    internal CancellationToken GetCurrentCancellationToken()
    {
        lock (syncRoot)
        {
            return cts?.Token ?? new(true);
        }
    }

    internal bool CanToggle(HotKeyParameter parameter)
    {
        return IsEnabled && Modifiers == parameter.Modifiers && Key == parameter.Key;
    }

    internal void Toggle(WaitCallback callback)
    {
        taskContext.InvokeOnMainThread(() =>
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
                    ThreadPool.QueueUserWorkItem(callback, this);
                    IsOn = true;
                }
            }
        });
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

        registered = !UnregisterHotKey(messageHwnd, hotKeyId);
        if (!registered)
        {
            RegisteredHotKeys.TryRemove(Parameter, out _);
        }

        if (IsOn)
        {
            Toggle(default!);
        }

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

    private unsafe void SaveModifiersAndKeyThenRefresh()
    {
        HotKeyParameter current = Parameter;
        Debug.Assert(sizeof(HotKeyParameter) == sizeof(int));
        LocalSetting.Set(settingKey, *(int*)&current);

        Unregister();
        Register();
    }
}