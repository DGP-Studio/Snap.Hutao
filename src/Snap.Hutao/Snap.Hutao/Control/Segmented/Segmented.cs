// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace Snap.Hutao.Control.Segmented;

internal partial class Segmented : ListViewBase
{
    private int correctSelectedIndex = -1;

    public Segmented()
    {
        DefaultStyleKey = typeof(Segmented);

        RegisterPropertyChangedCallback(SelectedIndexProperty, OnSelectedIndexChanged);
    }

    protected override DependencyObject GetContainerForItemOverride() => new SegmentedItem();

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is SegmentedItem;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (!IsLoaded)
        {
            SelectedIndex = correctSelectedIndex;
        }

        PreviewKeyDown -= OnPreviewKeyDown;
        PreviewKeyDown += OnPreviewKeyDown;
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);
        if (element is SegmentedItem segmentedItem)
        {
            segmentedItem.Loaded += OnLoaded;
        }
    }

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
    }

    private void OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.Left:
                e.Handled = MoveFocus(true);
                break;
            case VirtualKey.Right:
                e.Handled = MoveFocus(false);
                break;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is SegmentedItem segmentedItem)
        {
            segmentedItem.Loaded -= OnLoaded;
        }
    }

    private bool MoveFocus(bool reverse)
    {
        SegmentedItem? currentContainerItem = GetCurrentContainerItem();

        if (currentContainerItem is null)
        {
            return false;
        }

        int previousIndex = Items.IndexOf(ItemFromContainer(currentContainerItem));

        if (reverse)
        {
            if (previousIndex > 0 && ContainerFromIndex(previousIndex - 1) is SegmentedItem newItem)
            {
                newItem.Focus(FocusState.Keyboard);
                return true;
            }
        }
        else
        {
            if (previousIndex < Items.Count - 1 && ContainerFromIndex(previousIndex + 1) is SegmentedItem newItem)
            {
                newItem.Focus(FocusState.Keyboard);
                return true;
            }
        }

        return false;
    }

    private SegmentedItem? GetCurrentContainerItem()
    {
        if (XamlRoot is not null)
        {
            return (SegmentedItem)FocusManager.GetFocusedElement(XamlRoot);
        }
        else
        {
            return (SegmentedItem)FocusManager.GetFocusedElement();
        }
    }

    private void OnSelectedIndexChanged(DependencyObject sender, DependencyProperty dp)
    {
        // https://github.com/microsoft/microsoft-ui-xaml/issues/8257
        if (correctSelectedIndex == -1 && SelectedIndex > -1)
        {
            correctSelectedIndex = SelectedIndex;
        }
    }
}
