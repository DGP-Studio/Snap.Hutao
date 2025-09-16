// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.Data;

internal partial class ImmutableArrayCurrentItemPair<T> : ObservableObject
{
    public ImmutableArrayCurrentItemPair(ImmutableArray<T> array)
    {
        Array = array;
    }

    public ImmutableArrayCurrentItemPair(ImmutableArray<T> array, T? currentItem)
        : this(array)
    {
        CurrentItem = currentItem;
    }

    public ImmutableArray<T> Array { get; }

    [ObservableProperty]
    public partial T? CurrentItem { get; set; }
}