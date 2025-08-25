// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel.Wiki;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[DependencyProperty<BaseValueInfo>("BaseValueInfo")]
[DependencyProperty<bool>("IsPromoteVisible", DefaultValue = true, NotNull = true)]
internal sealed partial class BaseValueSlider : UserControl
{
    public BaseValueSlider()
    {
        InitializeComponent();
    }
}