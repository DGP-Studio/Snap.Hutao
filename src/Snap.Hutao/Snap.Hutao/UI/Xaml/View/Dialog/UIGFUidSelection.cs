// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

internal sealed partial class UIGFUidSelection : ObservableObject
{
    public UIGFUidSelection(uint uid)
    {
        Uid = uid;
    }

    [ObservableProperty]
    public partial bool IsSelected { get; set; } = true;

    public uint Uid { get; }
}