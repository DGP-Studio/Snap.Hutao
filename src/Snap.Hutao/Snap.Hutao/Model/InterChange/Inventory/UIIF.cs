// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal sealed class UIIF
{
    public const string CurrentVersion = "v1.0";

    private static readonly FrozenSet<string> SupportedVersion = [CurrentVersion];

    [JsonPropertyName("info")]
    public UIIFInfo Info { get; set; } = default!;

    [JsonPropertyName("list")]
    public List<UIIFItem> List { get; set; } = default!;

    public bool IsCurrentVersionSupported()
    {
        return SupportedVersion.Contains(Info.UIIFVersion);
    }
}