// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.GachaLog;

internal abstract class Wish
{
    public required string Name { get; init; }

    public required int TotalCount { get; init; }

    public string FormattedTimeSpan
    {
        get
        {
            if (From == DateTimeOffset.MaxValue && To == DateTimeOffset.MinValue)
            {
                return string.Empty;
            }

            return $"{From:yyyy.MM.dd} - {To:yyyy.MM.dd}";
        }
    }

    public string FormattedTotalCount
    {
        get => SH.FormatModelBindingGachaWishBaseTotalCount(TotalCount);
    }

    internal required DateTimeOffset From { get; init; }

    internal required DateTimeOffset To { get; init; }
}