// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Control.Selector;

internal sealed class InfoBarTemplateSelector : DataTemplateSelector
{
    public DataTemplate ActionButtonEnabled { get; set; } = default!;

    public DataTemplate ActionButtonDisabled { get; set; } = default!;

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is InfoBarOptions { ActionButtonContent: { }, ActionButtonCommand: { } })
        {
            return ActionButtonEnabled;
        }

        return ActionButtonDisabled;
    }
}
