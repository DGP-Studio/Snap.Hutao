// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.GachaLog;

namespace Snap.Hutao.Web.Endpoint;

internal interface IHomaGachaLogEndpoints : IHomaRootAccess
{
    public string GachaLogEndIds(string uid)
    {
        return $"{Root}/GachaLog/EndIds?Uid={uid}";
    }

    public string GachaLogRetrieve()
    {
        return $"{Root}/GachaLog/Retrieve";
    }

    public string GachaLogUpload()
    {
        return $"{Root}/GachaLog/Upload";
    }

    public string GachaLogEntries()
    {
        return $"{Root}/GachaLog/Entries";
    }

    public string GachaLogDelete(string uid)
    {
        return $"{Root}/GachaLog/Delete?Uid={uid}";
    }

    public string GachaLogStatisticsCurrentEvents()
    {
        return $"{Root}/GachaLog/Statistics/CurrentEventStatistics";
    }

    public string GachaLogStatisticsDistribution(GachaDistributionType distributionType)
    {
        return $"{Root}/GachaLog/Statistics/Distribution/{distributionType}";
    }
}