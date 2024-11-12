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
    private readonly LowLevelKeyOptions lowLevelKeyOptions;
    private readonly RuntimeOptions runtimeOptions;
    private readonly HotKeyOptions hotKeyOptions;

    public LowLevelKeyOptions LowLevelKeyOptions { get => lowLevelKeyOptions; }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public HotKeyOptions HotKeyOptions { get => hotKeyOptions; }
}