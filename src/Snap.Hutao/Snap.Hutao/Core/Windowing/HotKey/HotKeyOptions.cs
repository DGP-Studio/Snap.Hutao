// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing.HotKey;

[Injection(InjectAs.Singleton)]
internal sealed class HotKeyOptions : ObservableObject
{
    private bool isVirtualKeyF8Pressed;

    public bool IsMouseClickRepeatForeverOn { get => isVirtualKeyF8Pressed; set => SetProperty(ref isVirtualKeyF8Pressed, value); }
}