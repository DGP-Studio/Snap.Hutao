// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Complex;

internal class RateAndDelta
{
    public RateAndDelta(double rate, double? lastRate)
    {
        Rate = $"{rate:P3}";
        RateDelta = lastRate.TryGetValue(out double lastRateValue) ? FormatDelta(rate - lastRateValue) : string.Empty;
    }

    public string Rate { get; }

    public string RateDelta { get; }

    public static RateAndDelta Create((double Rate, double? LastRate) tuple)
    {
        return new RateAndDelta(tuple.Rate, tuple.LastRate);
    }

    public static RateAndDelta Create((double Rate, double LastRate) tuple)
    {
        return new RateAndDelta(tuple.Rate, tuple.LastRate);
    }

    private static string FormatDelta(double value)
    {
        return value switch
        {
            >= 0 => $"+{value:P3}",
            < 0 => $"{value:P3}",
            _ => string.Empty,
        };
    }
}