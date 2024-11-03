// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

internal sealed partial class CountdownCardTemplateSelector : DataTemplateSelector
{
    public DataTemplate AvatarCountdownTemplate { get; set; } = default!;

    public DataTemplate WeaponCountdownTemplate { get; set; } = default!;

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is not Countdown countdown)
        {
            throw HutaoException.NotSupported();
        }

        return countdown.Item is Avatar ? AvatarCountdownTemplate : WeaponCountdownTemplate;
    }
}