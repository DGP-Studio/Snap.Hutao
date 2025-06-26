// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class PredownloadStatus
{
    public PredownloadStatus(string tag, bool finished, int totalBlocks)
    {
        Tag = tag;
        Finished = finished;
        TotalBlocks = totalBlocks;
    }

    [JsonPropertyName("tag")]
    public string Tag { get; set; }

    [JsonPropertyName("finished")]
    public bool Finished { get; set; }

    [JsonPropertyName("total_blocks")]
    public int TotalBlocks { get; set; }
}