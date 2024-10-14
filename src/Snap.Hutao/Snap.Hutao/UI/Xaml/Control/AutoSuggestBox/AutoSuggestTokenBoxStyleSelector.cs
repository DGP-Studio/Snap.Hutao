// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

internal sealed partial class AutoSuggestTokenBoxStyleSelector : StyleSelector
{
    public Style TokenStyle { get; set; } = default!;

    public Style TextStyle { get; set; } = default!;

    /// <inheritdoc/>
    protected override Style SelectStyleCore(object item, DependencyObject container)
    {
        return item is ITokenStringContainer ? TextStyle : TokenStyle;
    }
}
