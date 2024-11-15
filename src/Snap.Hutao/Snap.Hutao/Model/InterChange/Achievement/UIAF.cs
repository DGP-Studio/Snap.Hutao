// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.Achievement;

internal sealed class UIAF
{
    public const string CurrentVersion = "v1.1";

    private static readonly FrozenSet<string> SupportedVersion = [CurrentVersion];

    [JsonRequired]
    [JsonPropertyName("info")]
    public UIAFInfo Info { get; set; } = default!;

    [JsonPropertyName("list")]
    public ImmutableArray<UIAFItem> List { get; set; }

    public bool IsCurrentVersionSupported()
    {
        return SupportedVersion.Contains(Info.UIAFVersion ?? string.Empty);
    }
}