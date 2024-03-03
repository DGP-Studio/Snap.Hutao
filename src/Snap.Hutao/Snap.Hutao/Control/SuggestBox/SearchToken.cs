// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.SuggestBox;

internal sealed partial class SearchToken : ISearchToken
{
    public SearchToken(string value)
    {
        Value = value;
    }

    public string Value { get; set; } = default!;

    public override string ToString()
    {
        return Value;
    }
}