// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

internal sealed partial class DataRowStackPanel : Microsoft.UI.Xaml.Controls.StackPanel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        Size size = base.MeasureOverride(availableSize);

        List<double> maxLengthList = [];

        foreach (UIElement element in Children)
        {
            if (element.Visibility is not Visibility.Visible || element is not IDataRow row)
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

        if (maxLengthList.Count > 0)
        {
            ImmutableArray<double> maxLengthArray = [.. maxLengthList];
            foreach (UIElement element in Children)
            {
                if (element.Visibility is not Visibility.Visible || element is not IDataRow row)
                {
                    continue;
                }

                row.ColumnsLength = maxLengthArray;
            }
        }

        return size;
    }
}