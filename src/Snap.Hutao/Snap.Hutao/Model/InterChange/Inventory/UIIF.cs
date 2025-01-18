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
    public UIIFInfo Info { get; set; } = default!;

    [JsonPropertyName("list")]
    public ImmutableArray<UIIFItem> List { get; set; } = default!;

    public bool IsCurrentVersionSupported()
    {
        return SupportedVersion.Contains(Info.UIIFVersion);
    }
}