// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

internal sealed partial class UIGFUidSelection : ObservableObject
{
    private bool isSelected = true;

    public UIGFUidSelection(uint uid)
    {
        Uid = uid;
    }

    public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }

    public uint Uid { get; set; }
}