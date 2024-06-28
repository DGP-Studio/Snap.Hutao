// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common.Deferred;

namespace Snap.Hutao.UI.Xaml.Control.TokenizingTextBox;

internal sealed class TokenItemRemovingEventArgs : DeferredCancelEventArgs
{
    public TokenItemRemovingEventArgs(object item, TokenizingTextBoxItem token)
    {
        Item = item;
        Token = token;
    }

    public object Item { get; private set; }

    public TokenizingTextBoxItem Token { get; private set; }
}
