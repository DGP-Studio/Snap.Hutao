// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;

namespace Snap.Hutao.Model.InterChange.Achievement;

internal sealed class UIAF
{
    public const string CurrentVersion = "v1.1";

    private static readonly FrozenSet<string> SupportedVersion = FrozenSet.ToFrozenSet([CurrentVersion]);

    [JsonRequired]
    [JsonPropertyName("info")]
    public UIAFInfo Info { get; set; } = default!;

    [JsonPropertyName("list")]
    public List<UIAFItem> List { get; set; } = default!;

    public bool IsCurrentVersionSupported()
    {
        return SupportedVersion.Contains(Info.UIAFVersion ?? string.Empty);
    }
}