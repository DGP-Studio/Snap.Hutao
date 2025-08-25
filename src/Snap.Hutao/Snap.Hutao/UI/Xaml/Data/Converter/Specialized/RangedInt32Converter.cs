// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

[DependencyProperty<int>("Minimum", DefaultValue = 0, NotNull = true)]
[DependencyProperty<int>("Maximum", DefaultValue = int.MaxValue, NotNull = true)]
internal sealed partial class RangedInt32Converter : DependencyValueConverter<int, int>
{
    public override int Convert(int from)
    {
        return Math.Clamp(from, Minimum, Maximum);
    }

    public override int ConvertBack(int to)
    {
        return Math.Clamp(to, Minimum, Maximum);
    }
}