// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;
using System.Collections;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

internal sealed partial class EmptyCollectionToVisibilityRevertConverter : EmptyObjectToObjectConverter
{
    public EmptyCollectionToVisibilityRevertConverter()
    {
        EmptyValue = Visibility.Visible;
        NotEmptyValue = Visibility.Collapsed;
    }

    protected override bool CheckValueIsEmpty(object value)
    {
        if (value is ICollection collection)
        {
            return collection.Count <= 0;
        }

        return base.CheckValueIsEmpty(value);
    }
}