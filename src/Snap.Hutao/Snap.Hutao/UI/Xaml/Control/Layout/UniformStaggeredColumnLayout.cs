// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.UI.Xaml.Control.Layout;

[DebuggerDisplay("Count = {Count}, Height = {Height}")]
internal sealed partial class UniformStaggeredColumnLayout : List<UniformStaggeredItem>
{
    public double Height { get; private set; }

    public new void Add(UniformStaggeredItem item)
    {
        Height = item.Top + item.Height;
        base.Add(item);
    }

    public new void Clear()
    {
        Height = 0;
        base.Clear();
    }
}