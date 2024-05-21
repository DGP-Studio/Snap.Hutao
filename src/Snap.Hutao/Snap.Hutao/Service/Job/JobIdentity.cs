// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Quartz;
using Snap.Hutao.Service.DailyNote;

namespace Snap.Hutao.Service.Job;

internal static class JobIdentity
{
    public const string DailyNoteGroupName = "DailyNote";
    public const string DailyNoteRefreshJobName = "RefreshJob";
    public const string DailyNoteRefreshTriggerName = "RefreshTrigger";
}