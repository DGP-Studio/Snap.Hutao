// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Segmented;

[DependencyProperty("Icon", typeof(IconElement), null, nameof(OnIconPropertyChanged))]
internal partial class SegmentedItem : ListViewItem
{
    private const string IconLeftState = "IconLeft";
    private const string IconOnlyState = "IconOnly";
    private const string ContentOnlyState = "ContentOnly";

    public SegmentedItem()
    {
        DefaultStyleKey = typeof(SegmentedItem);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        OnIconChanged();
        ContentChanged();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        ContentChanged();
    }

    private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((SegmentedItem)d).OnIconChanged();
    }

    private void ContentChanged()
    {
        if (Content is not null)
        {
            VisualStateManager.GoToState(this, IconLeftState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, IconOnlyState, true);
        }
    }

    private void OnIconChanged()
    {
        if (Icon is not null)
        {
            VisualStateManager.GoToState(this, IconLeftState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, ContentOnlyState, true);
        }
    }
}
