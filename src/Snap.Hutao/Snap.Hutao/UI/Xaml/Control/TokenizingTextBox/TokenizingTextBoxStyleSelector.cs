// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control.TokenizingTextBox;

internal class TokenizingTextBoxStyleSelector : StyleSelector
{
    public Style TokenStyle { get; set; } = default!;

    public Style TextStyle { get; set; } = default!;

    /// <inheritdoc/>
    protected override Style SelectStyleCore(object item, DependencyObject container)
    {
        if (item is ITokenStringContainer)
        {
            return TextStyle;
        }

        return TokenStyle;
    }
}
