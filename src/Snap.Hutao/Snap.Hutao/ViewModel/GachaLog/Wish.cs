// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.GachaLog;

internal abstract class Wish
{
    public string Name { get; set; } = default!;

    public int TotalCount { get; set; }

    public string TimeSpanFormatted
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

    public string TotalCountFormatted
    {
        get => SH.FormatModelBindingGachaWishBaseTotalCountFormat(TotalCount);
    }

    internal DateTimeOffset From { get; set; }

    internal DateTimeOffset To { get; set; }
}