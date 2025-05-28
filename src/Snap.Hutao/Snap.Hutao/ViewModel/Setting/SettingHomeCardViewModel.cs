// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;

namespace Snap.Hutao.ViewModel.Setting;

internal sealed class SettingHomeCardViewModel : ObservableObject
{
    private readonly string presentKey;
    private readonly string orderKey;

    public SettingHomeCardViewModel(string name, string presentKey, string orderKey)
    {
        Name = name;
        this.presentKey = presentKey;
        this.orderKey = orderKey;
    }

    public string Name { get; }

    public bool IsPresented
    {
        get => LocalSetting.Get(presentKey, true);
        set => LocalSetting.Set(presentKey, value);
    }

    public int Order
    {
        get => LocalSetting.Get(orderKey, 0);
        set => LocalSetting.Set(orderKey, value);
    }
}