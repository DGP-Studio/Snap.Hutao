// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class FrameRateStringConverter : ValueConverter<int, string>
{
    public override string Convert(int from)
    {
        if (from >= 0)
        {
            return from.ToString();
        }

        return "∞";
    }
}