// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.ViewModel.Game;

internal sealed partial class AspectRatioComboBoxTemplateSelector : DataTemplateSelector
{
    public DataTemplate ListTemplate { get; set; } = default!;

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        return container is ContentPresenter ? default : ListTemplate;
    }
}