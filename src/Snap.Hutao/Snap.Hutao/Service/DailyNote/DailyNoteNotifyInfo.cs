// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.DailyNote;

internal readonly struct DailyNoteNotifyInfo
{
    public readonly string Title;
    public readonly string AdaptiveIcon;
    public readonly string AdaptiveHint;
    public readonly string Hint;

    public DailyNoteNotifyInfo(string title, string adaptiveIcon, string adaptiveHint, string hint)
    {
        Title = title;
        AdaptiveIcon = adaptiveIcon;
        AdaptiveHint = adaptiveHint;
        Hint = hint;
    }
}