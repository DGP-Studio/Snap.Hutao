// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control.Panel.DataTable;

[DependencyProperty("CanResize", typeof(bool))]
[DependencyProperty("DesiredWidth", typeof(GridLength), default, nameof(OnDesiredWidthPropertyChanged), RawDefaultValue = "GridLength.Auto")]
internal sealed partial class DataColumn : ContentControl
{
    public DataColumn()
    {
        DefaultStyleKey = typeof(DataColumn);
    }

    internal double MaxChildDesiredWidth { get; set; }

    internal GridLength CurrentWidth { get; private set; }

    private static void OnDesiredWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // If the developer updates the size of the column, update our internal copy
        if (d is DataColumn column)
        {
            column.CurrentWidth = column.DesiredWidth;
        }
    }
}