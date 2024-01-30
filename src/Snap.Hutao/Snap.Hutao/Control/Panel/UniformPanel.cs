// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;

namespace Snap.Hutao.Control.Panel;

[DependencyProperty("MinItemWidth", typeof(double))]
internal sealed partial class UniformPanel : UniformGrid
{
    public UniformPanel()
    {
        Columns = 1;
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
    {
        Columns = (int)((e.NewSize.Width + ColumnSpacing) / (MinItemWidth + ColumnSpacing));
    }
}