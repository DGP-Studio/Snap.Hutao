// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control.Card;

internal sealed class CardReference
{
    public Button? Card { get; set; }

    public static CardReference Create(Button card)
    {
        return new() { Card = card };
    }
}