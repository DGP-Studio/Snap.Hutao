// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IHomaSpiralAbyssEndpoints : IHomaRootAccess
{
    public string RecordCheck(string uid)
    {
        return $"{Root}/Record/Check?Uid={uid}";
    }

    public string RecordRank(string uid)
    {
        return $"{Root}/Record/Rank?Uid={uid}";
    }

    public string RecordUpload()
    {
        return $"{Root}/Record/Upload";
    }

    public string StatisticsOverview(bool last = false)
    {
        return $"{Root}/Statistics/Overview?Last={last}";
    }

    public string StatisticsAvatarAttendanceRate(bool last = false)
    {
        return $"{Root}/Statistics/Avatar/AttendanceRate?Last={last}";
    }

    public string StatisticsAvatarUtilizationRate(bool last = false)
    {
        return $"{Root}/Statistics/Avatar/UtilizationRate?Last={last}";
    }

    public string StatisticsAvatarAvatarCollocation(bool last = false)
    {
        return $"{Root}/Statistics/Avatar/AvatarCollocation?Last={last}";
    }

    public string StatisticsAvatarHoldingRate(bool last = false)
    {
        return $"{Root}/Statistics/Avatar/HoldingRate?Last={last}";
    }

    public string StatisticsWeaponWeaponCollocation(bool last = false)
    {
        return $"{Root}/Statistics/Weapon/WeaponCollocation?Last={last}";
    }

    public string StatisticsTeamCombination(bool last = false)
    {
        return $"{Root}/Statistics/Team/Combination?Last={last}";
    }
}