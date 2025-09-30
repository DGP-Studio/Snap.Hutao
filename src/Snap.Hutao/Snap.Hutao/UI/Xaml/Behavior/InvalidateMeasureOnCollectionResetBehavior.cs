// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation.Collections;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Behavior;

[DependencyProperty<IObservableVector<object>>("ItemsSource", PropertyChangedCallbackName = nameof(OnItemsSourceChanged))]
internal sealed partial class InvalidateMeasureOnCollectionResetBehavior : BehaviorBase<ItemsRepeater>
{
    protected override bool Uninitialize()
    {
        ItemsSource?.VectorChanged -= OnVectorChanged;
        return base.Uninitialize();
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        InvalidateMeasureOnCollectionResetBehavior behavior = d.As<InvalidateMeasureOnCollectionResetBehavior>();

        if (e.OldValue is IObservableVector<object> oldVector)
        {
            oldVector.VectorChanged -= behavior.OnVectorChanged;
        }

        if (e.NewValue is IObservableVector<object> newVector)
        {
            newVector.VectorChanged += behavior.OnVectorChanged;
        }
    }

    private void OnVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
    {
        if (@event.CollectionChange is CollectionChange.Reset)
        {
            AssociatedObject?.InvalidateMeasure();
        }
    }
}