// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
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
}