// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.Achievement;

// This class unfortunately can't use required properties because it's been rooted in XamlTypeInfo
// ReSharper disable once InconsistentNaming
internal sealed class UIAF
{
    public const string CurrentVersion = "v1.1";

    private static readonly FrozenSet<string> SupportedVersion = [CurrentVersion];

    [JsonRequired]
    [JsonPropertyName("info")]
    public UIAFInfo Info { get; init; } = default!;

    [JsonPropertyName("list")]
    public ImmutableArray<UIAFItem> List { get; init; }

    public bool IsCurrentVersionSupported()
    {
        return SupportedVersion.Contains(Info.UIAFVersion ?? string.Empty);
    }
}