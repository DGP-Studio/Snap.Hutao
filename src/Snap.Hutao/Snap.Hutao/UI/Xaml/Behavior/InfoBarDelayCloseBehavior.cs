// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Behavior;

[SuppressMessage("", "CA1001")]
[DependencyProperty<int>("MilliSecondsDelay", NotNull = true)]
internal sealed partial class InfoBarDelayCloseBehavior : BehaviorBase<InfoBar>
{
    private readonly CancellationTokenSource userCloseTokenSource = new();

    protected override void OnAssociatedObjectLoaded()
    {
        AssociatedObject.Closed += OnInfoBarClosed;
        if (MilliSecondsDelay > 0)
        {
            PrivateDelayAsync().SafeForget();
        }
    }

    private async ValueTask PrivateDelayAsync()
    {
        try
        {
            await Task.Delay(MilliSecondsDelay, userCloseTokenSource.Token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        AssociatedObject?.IsOpen = false;
    }

    private void OnInfoBarClosed(InfoBar infoBar, InfoBarClosedEventArgs args)
    {
        if (args.Reason is InfoBarCloseReason.CloseButton)
        {
            userCloseTokenSource.Cancel();
        }

        AssociatedObject.Closed -= OnInfoBarClosed;
    }
}