// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

internal sealed class UIGFUidSelection : ObservableObject
{
    private bool isSelected = true;

    public UIGFUidSelection(string uid)
    {
        Uid = uid;
    }

    public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }

    public string Uid { get; set; }
}