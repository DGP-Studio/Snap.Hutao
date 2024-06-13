// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Behavior;

[DependencyProperty("MilliSecondsDelay", typeof(int))]
internal sealed partial class InfoBarDelayCloseBehavior : BehaviorBase<InfoBar>
{
    protected override void OnAssociatedObjectLoaded()
    {
        DelayCoreAsync().SafeForget();
    }

    private async ValueTask DelayCoreAsync()
    {
        if (MilliSecondsDelay > 0)
        {
            await Delay.FromMilliSeconds(MilliSecondsDelay).ConfigureAwait(true);
            if (AssociatedObject is not null)
            {
                AssociatedObject.IsOpen = false;
            }
        }
    }
}
