// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Miyolive;

internal sealed class CodeListWrapper
{
    [JsonPropertyName("code_list")]
    public required ImmutableArray<CodeWrapper> CodeList { get; init; }
}