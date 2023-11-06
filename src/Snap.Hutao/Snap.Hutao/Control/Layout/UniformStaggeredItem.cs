// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Hutao.Control.Layout;

internal sealed class UniformStaggeredItem
{
    public UniformStaggeredItem(int index)
    {
        Index = index;
    }

    public double Top { get; internal set; }

    public double Height { get; internal set; }

    public int Index { get; }

    public UIElement? Element { get; internal set; }
}