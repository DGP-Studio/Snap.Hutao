// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control.Panel.DataTable;

[DependencyProperty<bool>("CanResize", NotNull = true)]
[DependencyProperty<GridLength>("DesiredWidth", PropertyChangedCallbackName = nameof(OnDesiredWidthPropertyChanged), CreateDefaultValueCallbackName = nameof(CreateDesiredWidthDefaultValue), NotNull = true)]
internal sealed partial class DataColumn : ContentControl
{
    public DataColumn()
    {
        DefaultStyleKey = typeof(DataColumn);
    }

    internal double MaxChildDesiredWidth { get; set; }

    internal GridLength CurrentWidth { get; private set; }

    private static object CreateDesiredWidthDefaultValue()
    {
        return GridLength.Auto;
    }

    private static void OnDesiredWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // If the developer updates the size of the column, update our internal copy
        if (d is DataColumn column)
        {
            column.CurrentWidth = column.DesiredWidth;
        }
    }
}