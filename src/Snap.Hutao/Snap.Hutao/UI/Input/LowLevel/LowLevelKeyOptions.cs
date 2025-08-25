// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Collections.Immutable;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Snap.Hutao.UI.Input.LowLevel;

[SuppressMessage("", "SA1500")]
[SuppressMessage("", "SA1513")]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class LowLevelKeyOptions : ObservableObject
{
    public static ImmutableArray<NameValue<VIRTUAL_KEY>> VirtualKeys { get => Input.VirtualKeys.Values; }

    [UsedImplicitly]
    public NameValue<VIRTUAL_KEY> WebView2VideoPlayPauseKey
    {
        get;
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2VideoPlayPause, value.Value);
            }
        }
    } = Input.VirtualKeys.First(VirtualKeys, UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoPlayPause, VIRTUAL_KEY.VK__none_));

    [UsedImplicitly]
    public NameValue<VIRTUAL_KEY> WebView2VideoFastForwardKey
    {
        get;
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2VideoFastForward, value.Value);
            }
        }
    } = Input.VirtualKeys.First(VirtualKeys, UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoFastForward, VIRTUAL_KEY.VK__none_));

    [UsedImplicitly]
    public NameValue<VIRTUAL_KEY> WebView2VideoRewindKey
    {
        get;
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2VideoRewind, value.Value);
            }
        }
    } = Input.VirtualKeys.First(VirtualKeys, UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoRewind, VIRTUAL_KEY.VK__none_));

    [UsedImplicitly]
    public NameValue<VIRTUAL_KEY> WebView2HideKey
    {
        get;
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2Hide, value.Value);
            }
        }
    } = Input.VirtualKeys.First(VirtualKeys, UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2Hide, VIRTUAL_KEY.VK__none_));

    [UsedImplicitly]
    public NameValue<VIRTUAL_KEY> OverlayHideKey
    {
        get;
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardOverlayHide, value.Value);
            }
        }
    } = Input.VirtualKeys.First(VirtualKeys, UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardOverlayHide, VIRTUAL_KEY.VK__none_));
}