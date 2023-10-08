// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Core.Windowing.HotKey;

[Injection(InjectAs.Singleton)]
internal sealed class HotKeyOptions : ObservableObject
{
    private bool isVirtualKeyF8Pressed;

    public bool IsMouseClickRepeatForeverOn { get => isVirtualKeyF8Pressed; set => SetProperty(ref isVirtualKeyF8Pressed, value); }
}