// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Windows.Foundation;
using MUXCPanel = Microsoft.UI.Xaml.Controls.Panel;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

[DependencyProperty("AreChildrenDataRows", typeof(bool), false, nameof(OnAreChildrenDataRowsChanged), IsAttached = true, AttachedType = typeof(MUXCPanel))]
public static partial class PanelHelper
{
    private static void OnAreChildrenDataRowsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        MUXCPanel panel = (MUXCPanel)sender;
        bool newValue = (bool)e.NewValue;

        if (panel.IsLoaded)
        {
            SetLoadedPanelAreChildrenDataRows(panel, newValue);
            return;
        }

        panel.Loaded += SetPanelAreChildrenDataRows;
    }

    private static void SetPanelAreChildrenDataRows(object sender, RoutedEventArgs e)
    {
        MUXCPanel panel = (MUXCPanel)sender;
        bool value = GetAreChildrenDataRows(panel);
        SetLoadedPanelAreChildrenDataRows(panel, value);

        panel.Loaded -= SetPanelAreChildrenDataRows;
    }

    private static void SetLoadedPanelAreChildrenDataRows(MUXCPanel panel, bool value)
    {
        List<double> maxLengthList = [];

        foreach (UIElement element in panel.Children)
        {
            if (element is not IDataRow row)
            {
                continue;
            }

            ImmutableArray<double> columnsLength = row.ColumnsLength;

            maxLengthList.EnsureCapacity(columnsLength.Length);
            CollectionsMarshal.SetCount(maxLengthList, Math.Max(maxLengthList.Count, columnsLength.Length));

            Span<double> maxLengthSpan = CollectionsMarshal.AsSpan(maxLengthList);

            for (int index = 0; index < columnsLength.Length; index++)
            {
                double length = columnsLength[index];
                ref double maxLength = ref maxLengthSpan[index];
                maxLength = Math.Max(maxLength, length);
            }
        }

        ImmutableArray<double> maxLengthArray = [.. maxLengthList];
        foreach (UIElement element in panel.Children)
        {
            if (element is not IDataRow row)
            {
                continue;
            }

            row.ColumnsLength = maxLengthArray;
        }
    }
}