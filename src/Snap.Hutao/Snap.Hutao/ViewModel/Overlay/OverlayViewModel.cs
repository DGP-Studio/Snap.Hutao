// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.UI.Input.HotKey;

namespace Snap.Hutao.ViewModel.Overlay;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class OverlayViewModel : Abstraction.ViewModel
{
    public partial HotKeyOptions HotKeyOptions { get; }

    public partial RuntimeOptions RuntimeOptions { get; }
}