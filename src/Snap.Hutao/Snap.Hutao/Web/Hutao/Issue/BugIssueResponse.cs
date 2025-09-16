// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Issue;

internal sealed class BugIssueStats
{
    [JsonPropertyName("waiting_for_release")]
    public int WaitingForRelease { get; set; }

    [JsonPropertyName("untreated")]
    public int Untreated { get; set; }

    [JsonPropertyName("hard_to_fix")]
    public int HardToFix { get; set; }
}

internal sealed class BugIssuePayload
{
    [JsonPropertyName("details")]
    public List<BugIssueItem> Details { get; set; } = [];

    [JsonPropertyName("stat")]
    public BugIssueStats Stat { get; set; } = new();
}
