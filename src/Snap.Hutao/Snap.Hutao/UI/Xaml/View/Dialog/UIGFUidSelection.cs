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

    public bool IsSelected { get; set => SetProperty(ref field, value); } = true;

    public uint Uid { get; set; }
}