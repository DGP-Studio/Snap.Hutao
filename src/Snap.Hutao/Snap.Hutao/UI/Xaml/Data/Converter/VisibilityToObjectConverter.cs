﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

[DependencyProperty("VisibleValue", typeof(object))]
[DependencyProperty("CollapsedValue", typeof(object))]
internal sealed partial class VisibilityToObjectConverter : DependencyValueConverter<Visibility, object>
{
    public override object Convert(Visibility from)
    {
        return from is Visibility.Visible ? VisibleValue : CollapsedValue;
    }
}