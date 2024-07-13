// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

internal sealed class UIGFImportOptions
{
    public required Model.InterChange.UIGF UIGF { get; set; }

    public required HashSet<string> GachaArchiveUids { get; set; }

    public required HashSet<string> ReservedAchievementArchiveIdentities { get; set; }

    public required HashSet<string> ReservedAvatarInfoIdentities { get; set; }

    public required HashSet<string> ReservedCultivationProjectIdentities { get; set; }

    public required HashSet<string> ReservedSpiralAbyssIdentities { get; set; }
}