// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.RoleCombat.Post;

internal sealed class SimpleRoleCombatRecord
{
    public SimpleRoleCombatRecord(string uid, List<uint> backupAvatars, uint scheduleId)
    {
        Version = 1;
        Uid = uid;
        Identity = "Snap Hutao"; // hardcoded Identity name
        BackupAvatars = backupAvatars;
        ScheduleId = scheduleId;
    }

    public uint Version { get; set; }

    public string Uid { get; set; }

    public string Identity { get; set; }

    public List<uint> BackupAvatars { get; set; }

    public uint ScheduleId { get; set; }
}