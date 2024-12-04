// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Media.Backdrop;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class BackdropTypeToOpacityConverter : ValueConverter<BackdropType, double>
{
    public override double Convert(BackdropType from)
    {
        return from is BackdropType.None ? 1 : 0;
    }
}