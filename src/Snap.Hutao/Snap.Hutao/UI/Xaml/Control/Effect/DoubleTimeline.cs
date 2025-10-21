// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace Snap.Hutao.UI.Xaml.Control.Effect;

internal class DoubleTimeline
{
    private readonly TimelineProgressor progressor;

    public DoubleTimeline(double from = 0, double to = 1, double seconds = 1, TimeSpan? beginTime = null,
        bool autoReverse = true, bool forever = true, EasingFunctionBase? easingFunction = null)
    {
        progressor = new(seconds, autoReverse)
            { EasingFunction = easingFunction, BeginTime = beginTime, Forever = forever };
        From = from;
        To = to;
        Duration = new(TimeSpan.FromSeconds(seconds));
        AutoReverse = autoReverse;
        BeginTime = beginTime;
        Forever = forever;
    }

    public bool AutoReverse { get; }

    public TimeSpan? BeginTime { get; }

    public Duration Duration { get; }

    public bool Forever { get; }

    public double From { get; }

    public double To { get; }

    public double GetCurrentProgress(TimeSpan timeSpan)
    {
        double progress = progressor.GetCurrentProgress(timeSpan);
        return From + (To - From) * progress;
    }
}