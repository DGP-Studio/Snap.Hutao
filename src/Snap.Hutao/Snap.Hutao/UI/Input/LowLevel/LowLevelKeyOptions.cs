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
    public LowLevelKeyOptions()
    {
        VIRTUAL_KEY key = UnsafeLocalSetting.Get(SettingKeys.LowLevelKeyboardWebView2VideoPlayPause, VIRTUAL_KEY.VK__none_);
        WebView2VideoPlayPauseKey = VirtualKeys.GetList().Single(n => n.Value == key);
    }

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
    }
}