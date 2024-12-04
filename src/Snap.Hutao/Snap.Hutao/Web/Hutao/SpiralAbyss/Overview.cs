// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

internal sealed class Overview
{
    public int ScheduleId { get; set; }

    public int RecordTotal { get; set; }

    public int SpiralAbyssTotal { get; set; }

    public int SpiralAbyssPassed { get; set; }

    [JsonIgnore]
    public string SpiralAbyssPassedPercent { get => $"{(double)SpiralAbyssPassed / SpiralAbyssTotal:P2}"; }

    public int SpiralAbyssStarTotal { get; set; }

    [JsonIgnore]
    public string SpiralAbyssStarAverage { get => $"{(double)SpiralAbyssStarTotal / SpiralAbyssTotal:F2}"; }

    public int SpiralAbyssFullStar { get; set; }

    [JsonIgnore]
    public string SpiralAbyssFullStarPercent { get => $"{(double)SpiralAbyssFullStar / SpiralAbyssTotal:P2}"; }

    public long SpiralAbyssBattleTotal { get; set; }

    [JsonIgnore]
    public string SpiralAbyssBattleAverage { get => $"{(double)SpiralAbyssBattleTotal / SpiralAbyssTotal:F2}"; }

    public long Timestamp { get; set; }

    [JsonIgnore]
    public string RefreshTime { get => $"{DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).ToLocalTime():MM.dd HH:mm}"; }

    public double TimeTotal { get; set; }

    public double TimeAverage { get; set; }
}