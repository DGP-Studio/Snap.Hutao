// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.Control.Segmented;

[DependencyProperty("LeftItemMargin", typeof(Thickness))]
[DependencyProperty("MiddleItemMargin", typeof(Thickness))]
[DependencyProperty("RightItemMargin", typeof(Thickness))]
internal partial class SegmentedMarginConverter : DependencyObject, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        SegmentedItem segmentedItem = (SegmentedItem)value;
        ItemsControl listView = ItemsControl.ItemsControlFromItemContainer(segmentedItem);

        int index = listView.IndexFromContainer(segmentedItem);

        if (index == 0)
        {
            return LeftItemMargin;
        }
        else if (index == listView.Items.Count - 1)
        {
            return RightItemMargin;
        }
        else
        {
            return MiddleItemMargin;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}
