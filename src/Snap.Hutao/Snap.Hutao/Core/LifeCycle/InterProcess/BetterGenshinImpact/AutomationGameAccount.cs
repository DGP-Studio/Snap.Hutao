// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal sealed class AutomationGameAccount
{
    public required AutomationGameAccountType Type { get; set; }

    public required string NameOrUserId { get; set; }
}