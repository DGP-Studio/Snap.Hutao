// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Labs.WinUI.MarqueeTextRns;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Input;

namespace Snap.Hutao.Control.Behavior;

internal sealed class MarqueeTextBehavior : BehaviorBase<MarqueeText>
{
    private readonly PointerEventHandler pointerEnteredEventHandler;
    private readonly PointerEventHandler pointerExitedEventHandler;

    public MarqueeTextBehavior()
    {
        pointerEnteredEventHandler = OnPointerEntered;
        pointerExitedEventHandler = OnPointerExited;
    }

    protected override bool Initialize()
    {
        AssociatedObject.PointerEntered += pointerEnteredEventHandler;
        AssociatedObject.PointerExited += pointerExitedEventHandler;

        return true;
    }

    protected override bool Uninitialize()
    {
        AssociatedObject.PointerEntered -= pointerEnteredEventHandler;
        AssociatedObject.PointerExited -= pointerExitedEventHandler;

        return true;
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        AssociatedObject.StartMarquee();
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        AssociatedObject.StopMarquee();
    }
}