// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Input.HotKey;
using Snap.Hutao.UI.Input.LowLevel;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingHotKeyViewModel : Abstraction.ViewModel
{
    public partial LowLevelKeyOptions LowLevelKeyOptions { get; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public partial HotKeyOptions HotKeyOptions { get; }

    public int WebView2VideoFastForwardOrRewindSeconds
    {
        get => LocalSetting.Get(SettingKeys.WebView2VideoFastForwardOrRewindSeconds, 5);
        set => LocalSetting.Set(SettingKeys.WebView2VideoFastForwardOrRewindSeconds, value);
    }

    public double CompactWebView2WindowInactiveOpacity
    {
        get => LocalSetting.Get(SettingKeys.CompactWebView2WindowInactiveOpacity, 50D);
        set => LocalSetting.Set(SettingKeys.CompactWebView2WindowInactiveOpacity, value);
    }
}