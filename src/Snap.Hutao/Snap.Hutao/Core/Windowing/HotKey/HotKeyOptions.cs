// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Windows.System;

namespace Snap.Hutao.Core.Windowing.HotKey;

[Injection(InjectAs.Singleton)]
internal sealed partial class HotKeyOptions : ObservableObject
{
    private bool isMouseClickRepeatForeverOn;
    private HotKeyCombination mouseClickRepeatForeverKeyCombination;

    public HotKeyOptions(IServiceProvider serviceProvider)
    {
        mouseClickRepeatForeverKeyCombination = new(serviceProvider, SettingKeys.HotKeyMouseClickRepeatForever, 100000, default, VirtualKey.F8);
    }

    public List<NameValue<VirtualKey>> VirtualKeys { get; } = HotKey.VirtualKeys.GetList();

    public bool IsMouseClickRepeatForeverOn
    {
        get => isMouseClickRepeatForeverOn;
        set => SetProperty(ref isMouseClickRepeatForeverOn, value);
    }

    public HotKeyCombination MouseClickRepeatForeverKeyCombination
    {
        get => mouseClickRepeatForeverKeyCombination;
        set => SetProperty(ref mouseClickRepeatForeverKeyCombination, value);
    }
}