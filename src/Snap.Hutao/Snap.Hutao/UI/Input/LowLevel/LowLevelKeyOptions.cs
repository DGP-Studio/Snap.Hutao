// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

namespace Snap.Hutao.UI.Input.LowLevel;

[Injection(InjectAs.Singleton)]
internal sealed partial class LowLevelKeyOptions : ObservableObject
{
    private NameValue<VIRTUAL_KEY> webView2VideoPlayPauseKey;
    private NameValue<VIRTUAL_KEY> webView2VideoFastForwardKey;
    private NameValue<VIRTUAL_KEY> webView2VideoRewindKey;

    public LowLevelKeyOptions()
    {
        // TODO: once the VirtualKeys no longer duplicate, use Single instead of First
        VIRTUAL_KEY key = UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoPlayPause, VIRTUAL_KEY.VK__none_);
        webView2VideoPlayPauseKey = VirtualKeys.GetList().First(n => n.Value == key);

        key = UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoFastForward, VIRTUAL_KEY.VK__none_);
        webView2VideoFastForwardKey = VirtualKeys.GetList().First(n => n.Value == key);

        key = UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoRewind, VIRTUAL_KEY.VK__none_);
        webView2VideoRewindKey = VirtualKeys.GetList().First(n => n.Value == key);
    }

    public NameValue<VIRTUAL_KEY> WebView2VideoPlayPauseKey
    {
        get => webView2VideoPlayPauseKey;
        set
        {
            if (SetProperty(ref webView2VideoPlayPauseKey, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2VideoPlayPause, value.Value);
            }
        }
    }

    public NameValue<VIRTUAL_KEY> WebView2VideoFastForwardKey
    {
        get => webView2VideoFastForwardKey;
        set
        {
            if (SetProperty(ref webView2VideoFastForwardKey, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2VideoFastForward, value.Value);
            }
        }
    }

    public NameValue<VIRTUAL_KEY> WebView2VideoRewindKey
    {
        get => webView2VideoRewindKey;
        set
        {
            if (SetProperty(ref webView2VideoRewindKey, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.LowLevelKeyboardWebView2VideoRewind, value.Value);
            }
        }
    }
}