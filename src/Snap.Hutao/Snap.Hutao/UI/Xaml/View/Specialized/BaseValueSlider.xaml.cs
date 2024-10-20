// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel.Wiki;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[DependencyProperty("BaseValueInfo", typeof(BaseValueInfo))]
[DependencyProperty("IsPromoteVisible", typeof(bool), true)]
internal sealed partial class BaseValueSlider : UserControl
{
    public BaseValueSlider()
    {
        InitializeComponent();
    }
}