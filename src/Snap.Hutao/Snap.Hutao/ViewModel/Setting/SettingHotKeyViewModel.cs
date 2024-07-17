// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.UI.Input.HotKey;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingHotKeyViewModel : Abstraction.ViewModel
{
    private readonly RuntimeOptions runtimeOptions;
    private readonly HotKeyOptions hotKeyOptions;

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public HotKeyOptions HotKeyOptions { get => hotKeyOptions; }
}