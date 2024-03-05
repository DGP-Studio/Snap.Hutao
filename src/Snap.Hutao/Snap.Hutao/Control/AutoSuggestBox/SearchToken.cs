// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.UI;

namespace Snap.Hutao.Control.AutoSuggestBox;

internal sealed class SearchToken
{
    public SearchToken(SearchTokenKind kind, string value, Uri? iconUri = null, Uri? sideIconUri = null, Color? quality = null)
    {
        Value = value;
        Kind = kind;
        IconUri = iconUri;
        SideIconUri = sideIconUri;
        Quality = quality;
    }

    public SearchTokenKind Kind { get; }

    public string Value { get; set; } = default!;

    public Uri? IconUri { get; }

    public Uri? SideIconUri { get; }

    public Color? Quality { get; }

    public override string ToString()
    {
        return Value;
    }
}