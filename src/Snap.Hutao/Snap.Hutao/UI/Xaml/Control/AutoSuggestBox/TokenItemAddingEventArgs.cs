// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common.Deferred;

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

internal sealed class TokenItemAddingEventArgs : DeferredCancelEventArgs
{
    public TokenItemAddingEventArgs(string token)
    {
        TokenText = token;
    }

    public string TokenText { get; private set; }

    public object? Item { get; set; }
}