// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Yae.Achievement;

internal sealed class MethodRvaWrapper
{
    [JsonPropertyName("chinese")]
    public required MethodRva Chinese { get; init; }

    [JsonPropertyName("oversea")]
    public required MethodRva Oversea { get; init; }
}