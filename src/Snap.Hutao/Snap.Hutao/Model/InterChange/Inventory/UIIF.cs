// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal sealed class UIIF
{
    public const string CurrentVersion = "v1.0";

    private static readonly FrozenSet<string> SupportedVersion = [CurrentVersion];

    [JsonPropertyName("info")]
    public required UIIFInfo Info { get; init; }

    [JsonPropertyName("list")]
    public ImmutableArray<UIIFItem> List { get; init; }

    public bool IsCurrentVersionSupported()
    {
        return SupportedVersion.Contains(Info.UIIFVersion ?? string.Empty);
    }

    public UIIF WithList(ImmutableArray<UIIFItem> list)
    {
        return new()
        {
            Info = Info,
            List = list,
        };
    }
}