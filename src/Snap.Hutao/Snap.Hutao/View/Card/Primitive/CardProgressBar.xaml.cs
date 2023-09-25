// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Snap.Hutao.View.Card.Primitive;

[DependencyProperty("ProgressForeground", typeof(Brush))]
[DependencyProperty("TextForeground", typeof(Brush))]
[DependencyProperty("Maximum", typeof(double))]
[DependencyProperty("Value", typeof(double))]
[DependencyProperty("Header", typeof(string))]
[DependencyProperty("Description", typeof(string))]
internal sealed partial class CardProgressBar : Grid
{
    public CardProgressBar()
    {
        IAppResourceProvider appResourceProvider = Ioc.Default.GetRequiredService<IAppResourceProvider>();
        TextForeground = appResourceProvider.GetResource<Brush>("TextFillColorPrimaryBrush");
        InitializeComponent();
    }
}
