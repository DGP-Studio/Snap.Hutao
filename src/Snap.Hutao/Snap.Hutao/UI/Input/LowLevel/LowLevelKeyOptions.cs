// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Snap.Hutao.UI.Input.LowLevel;

[SuppressMessage("", "SA1500")]
[SuppressMessage("", "SA1513")]
[Injection(InjectAs.Singleton)]
internal sealed partial class LowLevelKeyOptions : ObservableObject
{
    [UsedImplicitly]
    public NameValue<VIRTUAL_KEY> WebView2VideoPlayPauseKey
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2VideoPlayPause, value.Value);
            }
        }
    } = VirtualKeys.First(UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoPlayPause, VIRTUAL_KEY.VK__none_));

    [UsedImplicitly]
    public NameValue<VIRTUAL_KEY> WebView2VideoFastForwardKey
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2VideoFastForward, value.Value);
            }
        }
    } = VirtualKeys.First(UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoFastForward, VIRTUAL_KEY.VK__none_));

    [UsedImplicitly]
    public NameValue<VIRTUAL_KEY> WebView2VideoRewindKey
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2VideoRewind, value.Value);
            }
        }
    } = VirtualKeys.First(UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoRewind, VIRTUAL_KEY.VK__none_));

    [UsedImplicitly]
    public NameValue<VIRTUAL_KEY> WebView2HideKey
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2Hide, value.Value);
            }
        }
    } = VirtualKeys.First(UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2Hide, VIRTUAL_KEY.VK__none_));
}