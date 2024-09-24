// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty("SystemBackdrop", typeof(SystemBackdrop), default(SystemBackdrop), nameof(OnSystemBackdropChanged), IsAttached = true, AttachedType = typeof(ComboBox))]
public sealed partial class ComboBoxHelper
{
    private static void OnSystemBackdropChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not ComboBox comboBox)
        {
            return;
        }

        if (!comboBox.IsLoaded)
        {
            comboBox.DropDownOpened += OnComboBoxDropDownOpened;
            return;
        }
    }

    private static void OnComboBoxDropDownOpened(object? sender, object e)
    {
        if (sender is not ComboBox comboBox)
        {
            return;
        }

        comboBox.Loaded -= OnComboBoxDropDownOpened;
        SetSystemBackdrop(comboBox);
    }

    private static void SetSystemBackdrop(ComboBox comboBox)
    {
        Popup? popup = comboBox.FindDescendant<Popup>(p => p.Name == "Popup");
        Debug.Assert(popup is not null);
        popup.SystemBackdrop = GetSystemBackdrop(comboBox);

        Border? popupBorder = popup.Child.FindDescendant<Border>(b => b.Name == "PopupBorder");
        Debug.Assert(popupBorder is not null);
        popupBorder.Background = new SolidColorBrush(Colors.Transparent);
    }
}