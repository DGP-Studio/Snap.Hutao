// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class TimestampToLocalTimeStringConverter : ValueConverter<long, string>
{
    public override string Convert(long from)
    {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(from).ToLocalTime();
        return $"{dto:yyyy-MM-dd HH:mm:ss}";
    }
}