// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Windows.Foundation.Collections;

namespace Snap.Hutao.Control.Collection.Alternating;

[DependencyProperty("ItemAlternateBackground", typeof(Microsoft.UI.Xaml.Media.Brush))]
internal sealed partial class AlternatingItemsControl : ItemsControl
{
    private readonly VectorChangedEventHandler<object> itemsVectorChangedEventHandler;

    public AlternatingItemsControl()
    {
        itemsVectorChangedEventHandler = OnItemsVectorChanged;
        Items.VectorChanged += itemsVectorChangedEventHandler;
    }

    private void OnItemsVectorChanged(IObservableVector<object> items, IVectorChangedEventArgs args)
    {
        if (args.CollectionChange is CollectionChange.Reset)
        {
            int index = (int)args.Index;
            for (int i = index; i < items.Count; i++)
            {
                if (items[i] is IAlternatingItem item)
                {
                    item.Background = i % 2 is 0 ? default : ItemAlternateBackground;
                }
                else
                {
                    break;
                }
            }
        }
    }
}