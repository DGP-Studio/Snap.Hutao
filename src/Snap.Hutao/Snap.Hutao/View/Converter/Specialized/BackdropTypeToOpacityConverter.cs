// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Windowing;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.View.Converter.Specialized;

internal sealed class BackdropTypeToOpacityConverter : ValueConverter<BackdropType, double>
{
    public override double Convert(BackdropType from)
    {
        return from is BackdropType.None ? 1 : 0;
    }
}