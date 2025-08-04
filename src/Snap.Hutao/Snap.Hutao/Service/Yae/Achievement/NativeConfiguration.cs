// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Service.Yae.Achievement;

internal sealed class NativeConfiguration
{
    public required int StoreCmdId { get; init; }

    public required int AchievementCmdId { get; init; }

    public required ImmutableDictionary<uint, MethodRva> MethodRva { get; init; }
}