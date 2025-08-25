// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.View;

internal sealed partial class TitleView : UserControl
{
    public TitleView()
    {
        InitializeComponent();
    }

    public FrameworkElement DragArea { get => DraggableGrid; }

    public ImmutableArray<FrameworkElement> Passthrough { get; } = [];
}