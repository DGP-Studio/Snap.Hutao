// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Data;

internal interface IAdvancedCollectionViewItem
{
    object? GetPropertyValue(string name);
}