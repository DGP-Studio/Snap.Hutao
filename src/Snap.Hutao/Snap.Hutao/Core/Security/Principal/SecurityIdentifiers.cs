// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Security.Principal;

namespace Snap.Hutao.Core.Security.Principal;

internal static class SecurityIdentifiers
{
    public static SecurityIdentifier Everyone { get; } = new(WellKnownSidType.WorldSid, null);
}