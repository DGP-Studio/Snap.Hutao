// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

internal sealed class UIGFExportOptions
{
    public string FilePath { get; set; } = default!;

    public List<Guid> GachaArchiveIds { get; set; } = [];

    public List<Guid> ReservedAchievementArchiveIds { get; set; } = [];

    public List<string> ReservedAvatarInfoUids { get; set; } = [];

    public List<Guid> ReservedCultivationProjectIds { get; set; } = [];

    public List<string> ReservedSpiralAbyssUids { get; set; } = [];

    public List<Guid> ReservedUserIds { get; set; } = [];
}