// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

[Flags]
internal enum AutomationGameAccountType
{
    UseRegistry = 0B0000,
    UseAuthTicket = 0B0001,

    Chinese = 0B0000,
    Oversea = 0B0010,

    RegistryChinese = UseRegistry | Chinese,
    RegistryOversea = UseRegistry | Oversea,
    AuthTicketChinese = UseAuthTicket | Chinese,
    AuthTicketOversea = UseAuthTicket | Oversea,
}