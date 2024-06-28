// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.TokenizingTextBox;

internal interface ITokenStringContainer
{
    string Text { get; set; }

    bool IsLast { get; }
}
