// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.UI.Input.HotKey;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class HotKeyOptions : ObservableObject, IDisposable
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private bool isDisposed;

    [GeneratedConstructor]
    public partial HotKeyOptions(IServiceProvider serviceProvider);

    static unsafe HotKeyOptions()
    {
        HutaoNativeHotKeyAction.InitializeBeforeSwitchCallback(HutaoNativeHotKeyBeforeSwitchCallback.Create(&HandleShouldPreventSwitch));
    }

    public static bool IsInGameOnly
    {
        get => LocalSetting.Get(SettingKeys.HotKeyRepeatForeverInGameOnly, false);
        set => LocalSetting.Set(SettingKeys.HotKeyRepeatForeverInGameOnly, value);
    }

    public static ImmutableArray<NameValue<VIRTUAL_KEY>> VirtualKeys { get => Input.VirtualKeys.HotKeyValues; }

    [ObservableProperty]
    public partial HotKeyCombination? MouseClickRepeatForeverKeyCombination { get; set; }

    [ObservableProperty]
    public partial HotKeyCombination? KeyPressRepeatForeverKeyCombination { get; set; }

    public async ValueTask InitializeAsync()
    {
        await taskContext.SwitchToMainThreadAsync();

        MouseClickRepeatForeverKeyCombination = new(serviceProvider, HutaoNativeHotKeyActionKind.MouseClickRepeatForever, SettingKeys.HotKeyMouseClickRepeatForever);
        KeyPressRepeatForeverKeyCombination = new(serviceProvider, HutaoNativeHotKeyActionKind.KeyPressRepeatForever, SettingKeys.HotKeyKeyPressRepeatForever);

        MouseClickRepeatForeverKeyCombination.Initialize();
        KeyPressRepeatForeverKeyCombination.Initialize();
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref isDisposed, true))
        {
            return;
        }

        MouseClickRepeatForeverKeyCombination?.Dispose();
        KeyPressRepeatForeverKeyCombination?.Dispose();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static BOOL HandleShouldPreventSwitch()
    {
        // This callback should always be called by the internal wndproc, so we are on the main thread.
        return IsInGameOnly && !GameLifeCycle.IsGameRunningRequiresMainThread();
    }
}